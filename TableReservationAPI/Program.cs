using ApplicationContext;
using ApplicationService.CustomJsonConverter;
using ApplicationService.HostedServices;
using ApplicationService.Mapper;
using ApplicationService.Models;
using ApplicationService.Models.JwtModels;
using ApplicationService.Services;
using ApplicationService.Services.Implementation;
using ApplicationService.Tasks;
using ApplicationService.Tasks.Implementation;
using ApplicationService.UnitOfWork;
using ApplicationService.UnitOfWork.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TableReservationAPI.CustomMiddleware;
using TableReservationAPI.Startup;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(null)
        )
    .AddJsonOptions
               (
                   x =>
                   {
                       x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                       x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                       x.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
                       x.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
                       x.JsonSerializerOptions.Converters.Add(new StringEnumConverter<EnumModel.ReservationStatus>());
                       x.JsonSerializerOptions.Converters.Add(new StringEnumConverter<EnumModel.Role>());
                   }  
               ) ;

var oauthGGConfig = builder.Configuration.GetRequiredSection("OAuthWebClient");
builder.Services.Configure<OAuthConfiguration>(oauthGGConfig);

//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//    }).AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false,
//            ValidateIssuerSigningKey = false

//        };
//    });
///JWT BEARER CONFIG
//var jwtConfig = builder.Configuration.GetSection("JwtConfiguration");

//if (jwtConfig != null)
//{
//    Console.WriteLine("Issuer: " + jwtConfig["Issuer"]);
//    Console.WriteLine("Audience: " + jwtConfig["Audience"]);
//    Console.WriteLine("Key:" + jwtConfig["Key"]);
//    builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//    }).AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = false,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwtConfig["Issuer"],
//            ValidAudience = jwtConfig["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"])),

//        };
//    });
//    builder.Services.Configure<JwtConfiguration>(jwtConfig);
//}
//else
//{
//    throw new Exception("Cannot initialize jwt bearer");
//}

builder.Services.Configure<Domain>(builder.Configuration.GetSection("Domain"));
builder.Services.AddDbContext<TableReservationContext>(option => option.UseNpgsql(Global.ConnectionString));
builder.Services.AddAutoMapper(typeof(Mapper));
AddServices.ConfiguringServices(builder);
AddServices.ConfiguringCors(builder);

builder.Services.AddHostedService<CheckinLateDetectionService>();


string guide_navToken = "Use the token retrieved from <a href=\"https://williamtrung.github.io/TableReservationClient/\" target=\"_blank\">Go to token credentials</a>";
string guide_toPostman = "Supply the token to postman Authorization - Type: Bearer Token";
string guide_roleAlert = "Default role: Customer; mail fpt.edu.vn: Reception; For role: Administrator - contact developer";
string current_version = "v1.3.3";
string br = "<br/>";
string v_110 = br + br + "v1.1.0" + " - Implement profile management";
string v_120 = br + br + "v1.2.0" + " - Implement auto trigger event on checkin late and checkout late";
string v_121 = br + br + "v1.2.1" + " - Fix return model - get profile information";
string v_130 = br + br + "v1.3.0" + " - Implement anonymous reservation booking for reception";
string v_131 = br + br + "v1.3.1" + " - Implement checkin checkout for reception";
string v_132 = br + br + "v1.3.2" + " - Fix reception/get-vacants to work properly --> passed in the desired model --> return tables' information";
string v_133 = br + br + "v1.3.3" + " - Fix manage-table to add table working properly --> passed in the status as 0: Available, 1: Unavailable";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(current_version, new OpenApiInfo
    {
        Title = "Table Reservation API " + current_version,
        Version = current_version,
        Description = guide_navToken + br + guide_toPostman + br + guide_roleAlert +
            v_110 + v_120 + v_121 + v_130 + v_131 + v_132 + v_133
    });    
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/"+current_version+"/swagger.json", "Table Reservation API " + current_version);
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the root URL
        c.DefaultModelRendering(ModelRendering.Model); // Use the model visualization for the default model rendering
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("CorsPolicy");

//AddHostedService.AddLateDetectionService(app);

app.Run();
//app.Run();