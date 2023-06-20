using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.ReservationModels
{
    public class VacantTables
    {
        public int Amount { get; }
        public TimeOnly Time { get; }
        public VacantTables(int amount, TimeOnly time)
        {
            Amount = amount;    
            Time = time;
        }
    }
}
