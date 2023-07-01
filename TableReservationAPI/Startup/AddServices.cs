using ApplicationService.Services.Implementation;
using ApplicationService.Services;
using ApplicationService.Tasks.Implementation;
using ApplicationService.Tasks;
using ApplicationService.UnitOfWork.Implementation;
using ApplicationService.UnitOfWork;

namespace TableReservationAPI.Startup
{
    public class AddServices
    {
        public static void ConfiguringServices(in WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<ITableManagementService, TableManagement>();
            builder.Services.AddTransient<IBookTableService, BookTableService>();
            builder.Services.AddTransient<IReceptionService, ReceptionService>();
            builder.Services.AddTransient<IGoogleService, GoogleService>();
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IAnonymousBookingService, AnonymousBookingService>();
            builder.Services.AddTransient<ITasks, Tasks>();
        }
        public static void ConfiguringCors(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }
    }
}
