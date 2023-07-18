using ApplicationCore.Entities;
using ApplicationCore.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using ApplicationService.Helper;

namespace ApplicationService.Models.ReservationModels
{
    public class NewAnonymousModel
    {
        [Required]
        [Range(1, 12)]
        public int GuestAmount { get; set; }
        [DesiredDateValidator]
        public DateOnly DesiredDate { get; set; }
        [Required]
        [TimeValidator]
        public TimeOnly DesiredTime { get; set; }
        public bool Private { get; set; } = false;
        [Required]
        [PhoneValidator(ErrorMessage = "Provided phone number is not valid!")]
        public string Phone { get; set; } = null!;
        public string? Note { get; set; }
        
        public Reservation ToReservation()
        {
            return new Reservation
            {
                GuestAmount = GuestAmount,
                Note = AnonymousReservationHelper.MergePhone_Note(Phone, Note),
                Private = Private,
                ReservedTime = DesiredDate.ToDateTime(DesiredTime),
            };
        }
        public NewReservationModel ToNewReservation()
        {
            return new NewReservationModel
            {
                Note = AnonymousReservationHelper.MergePhone_Note(Phone, Note),
                DesiredDate = DesiredDate,
                DesiredTime = DesiredTime,
                Private = Private,
                Seat = GuestAmount,                
            };
        }
    }
}
