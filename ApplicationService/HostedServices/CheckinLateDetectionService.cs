using ApplicationCore.Entities;
using ApplicationCore.Enum;
using ApplicationService.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.HostedServices
{
    public class CheckinLateDetectionService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _startTime;
        private readonly TimeSpan _endTime;

        public CheckinLateDetectionService(IServiceScopeFactory scopeFactory)
        {
            Console.WriteLine("Init CheckInLateDetectionService");            
            _scopeFactory = scopeFactory;
            _startTime = TimeSpan.FromHours(GlobalValidation.START_TIME);
            _endTime = TimeSpan.FromHours(GlobalValidation.END_TIME);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.Now.TimeOfDay;

                // Check if the current time is within the specified time frame
                if (currentTime >= _startTime && currentTime <= _endTime)
                {
                    Console.WriteLine("Start CheckInLateDetectionService");
                    using (var _tasks = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITasks>())
                    {
                        await _tasks.LateCheckInReservation();
                        await _tasks.LateCheckOutReservation();
                    }                    
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
