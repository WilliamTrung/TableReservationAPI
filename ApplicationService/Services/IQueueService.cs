using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IQueueService
    {
        Task ScheduleReservationCheckin(int reservationId, DateTime current_modified, DateTime executeTime);
        Task ScheduleReservationCheckout(int reservationId, DateTime current_modified, DateTime executeTime);
    }
}
