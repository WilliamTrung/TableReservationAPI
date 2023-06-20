using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.ReservationModels
{
    public class CustomerModifiedReservationModel
    {
        [Required]
        public int Id { get; set; }
        [Range(1, 12)]
        public int Seat { get; set; }
        public bool Private { get; set; }
        [DesiredDateValidator]
        public DateOnly DesiredDate { get; set; }
        [TimeValidator]
        public TimeOnly DesiredTime { get; set; }
        public string? Note { get; set; }

        public NewReservationModel ToNewReservationModel()
        {
            return new NewReservationModel
            {
                Seat = Seat,
                Private = Private,
                DesiredDate = DesiredDate,
                DesiredTime = DesiredTime,
                Note = Note
            };
        }

    }
}
