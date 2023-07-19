using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator
{
    public class GlobalValidation
    {
        public static readonly int CHECKIN_BOUNDARY = 30;//min
        public static readonly int CHECKOUT_MAX = 150; //min
        public static readonly int START_TIME = 9;//hour
        public static readonly int END_TIME = 21;//hour
        public static readonly int DEADLINE_HOURS = 0;//hour 3
        public static readonly int BOUNDARY_HOURS = 1;//hour
        public static readonly int BOUNDARY_SEAT = 1;//seat
    }
}