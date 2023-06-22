using ApplicationContext;
using ApplicationService.Mapper;
using ApplicationService.Models;
using ApplicationService.Models.JwtModels;
using ApplicationService.Services;
using ApplicationService.Services.Implementation;
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
using System.Text.Json.Serialization;

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
                   x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
               );

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
builder.Services.AddTransient<ILoginService, LoginService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Table Reservation API v1");
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the root URL
        c.DefaultModelRendering(ModelRendering.Model); // Use the model visualization for the default model rendering
        c.DocExpansion(DocExpansion.None); // Collapse all Swagger UI sections by default
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("CorsPolicy");
app.Run();