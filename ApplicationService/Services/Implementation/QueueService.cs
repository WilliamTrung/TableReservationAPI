using ApplicationService.Tasks;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class QueueService : IQueueService
    {
        private readonly ITasks _tasks;
        public QueueService(ITasks tasks)
        {
            _tasks = tasks;
        }

        public async Task ScheduleReservationCheckin(int reservationId, DateTime current_modified, DateTime executeTime)
        {
            Console.WriteLine("Reservation checkin for: "+ reservationId + " execute at " + executeTime + " : modified at " + current_modified);
            await ScheduleTask(executionTime: executeTime, task: _tasks.LateCheckInReservation(reservationId, current_modified));
        }

        public async Task ScheduleReservationCheckout(int reservationId, DateTime current_modified, DateTime executeTime)
        {
            Console.WriteLine("Reservation checkout for: " + reservationId + " execute at " + executeTime + " : modified at " + current_modified);
            await ScheduleTask(executionTime: executeTime, task: _tasks.LateCheckOutReservation(reservationId, current_modified));
        }

        public async Task ScheduleTask(DateTime executionTime, Task task)
        {
            TimeSpan delay = executionTime - DateTime.Now;

            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay);
                await task;
            }
        }
        
    }
}
