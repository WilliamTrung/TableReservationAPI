using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.ReservationModels
{
    public class NewReservationModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Range(1, 12)]
        public int Seat { get; set; } = 1;
        public bool Private { get; set; } = false;
        [DesiredDateValidator]
        public DateOnly DesiredDate { get; set; } 
        [Required]
        [TimeValidator]
        public TimeOnly DesiredTime { get; set; } 

        public string? Note { get; set; }   
        public Reservation ToReservation()
        {
            return new Reservation
            {                
                GuestAmount = Seat,
                Note = Note,
                ReservedTime = new DateTimeOffset(DesiredDate.ToDateTime(DesiredTime), TimeSpan.Zero),
                User = new User
                {
                    Email = Email
                },
                Private = Private
            };
        }    
        public DesiredReservationModel ToDesiredModel()
        {
            return new DesiredReservationModel
            {
                DesiredDate = DesiredDate,
                Private = Private,
                Seat = Seat,
            };
        }
    }
}
