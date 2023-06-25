using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models
{
    public class EnumModel
    {
        public enum Role
        {
            Administrator,
            Customer,
            Reception
        }
        public enum ReservationStatus
        {
            Pending,
            Assigned,
            Active,
            Cancel,
            Complete
        }
    }
}
