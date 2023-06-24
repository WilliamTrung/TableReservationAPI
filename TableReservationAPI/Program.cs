using ApplicationContext;
using ApplicationService.CustomJsonConverter;
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
builder.Services.AddCors(option =>
{
    option.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.Configure<Domain>(builder.Configuration.GetSection("Domain"));
builder.Services.AddDbContext<TableReservationContext>(option => option.UseNpgsql(Global.ConnectionString));
builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ITableManagementService, TableManagement>();
builder.Services.AddTransient<IBookTableService, BookTableService>();
builder.Services.AddTransient<IReceptionService, ReceptionService>();
builder.Services.AddTransient<IGoogleService, GoogleService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IQueueService, QueueService>();
builder.Services.AddTransient<ITasks, Tasks>();

string guide_navToken = "Use the token retrieved from <a href=\"https://williamtrung.github.io/TableReservationClient/\" target=\"_blank\">Go to token credentials</a>";
string guide_toPostman = "Supply the token to postman Authorization - Type: Bearer Token";
string guide_roleAlert = "Default role: Customer; mail fpt.edu.vn: Reception; For role: Administrator - contact developer";
string current_version = "v1.2.0";
string br = "<br/>";
string v_110 = br + br + "v1.1.0" + " - Implement profile management";
string v_120 = br + br + "v1.2.0" + " - Implement auto trigger event on checkin late and checkout late";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(current_version, new OpenApiInfo
    {
        Title = "Table Reservation API " + current_version,
        Version = current_version,
        Description = guide_navToken + br + guide_toPostman + br + guide_roleAlert +
            v_110 + v_120
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
app.Run();