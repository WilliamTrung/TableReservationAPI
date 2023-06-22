using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Enum
{
    public class IEnum
    {
        public enum TableStatus
        {
            Available,
            Unavailable
        }
        public enum ReservationStatus
        {
            Pending,
            Assigned,
            Active,
            Cancel,
            Complete
        }
        public enum Role
        {
            Administrator,
            Customer,
            Reception
        }
    }
}
