using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.ReservationModels
{
    public class DesiredReservationModel
    {
        [Range(1, 12)]
        public int Seat { get; set; } = 1;
        public bool Private { get; set; } = false;
        [DesiredDateValidator]
        public DateOnly DesiredDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
