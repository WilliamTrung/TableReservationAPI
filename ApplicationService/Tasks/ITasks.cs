using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Tasks
{
    public interface ITasks : IDisposable
    {
        Task LateCheckInReservation();
        Task LateCheckOutReservation();
    }
}
