using ApplicationService.HostedServices;

namespace TableReservationAPI.Startup
{
    public class AddHostedService
    {
        public static async void AddLateDetectionService (WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var lateDetectionService = services.GetRequiredService<IHostedService>();
                await lateDetectionService.StartAsync(CancellationToken.None);
            }
        }
    }
}
