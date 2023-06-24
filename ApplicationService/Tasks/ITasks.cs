using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Tasks
{
    public interface ITasks
    {
        Task LateCheckInReservation(int reservationId, DateTime current_modified);
        Task LateCheckOutReservation(int reservationId, DateTime current_modified);
    }
}
