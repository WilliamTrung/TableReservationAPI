using ApplicationCore.Entities;
using ApplicationCore.Enum;
using ApplicationService.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.ReservationModels
{
    public class UpdateAnonymousModel
    {
        public int Id { get; set; }
        public int? TableId { get; set; }
        public EnumModel.ReservationStatus Status { get; set; } = EnumModel.ReservationStatus.Pending;
        [Required]
        [Range(1, 12)]
        public int GuestAmount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        [Required]
        public DateTime ReservedTime { get; set; }
        public bool Private { get; set; } = false;
        [Required]
        [PhoneValidator]
        public string Phone { get; set; } = null!;
        public string? Note { get; set; }




        public UpdateAnonymousModel FromReservation(Reservation reservation)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            string[] split = AnonymousReservationHelper.SplitNoteToPhone_Note(reservation.Note);
#pragma warning restore CS8604 // Possible null reference argument.
            return new UpdateAnonymousModel
            {
                Id = reservation.Id,
                Status = (EnumModel.ReservationStatus)Enum.Parse(typeof(EnumModel.ReservationStatus), reservation.Status.ToString()),
                GuestAmount = reservation.GuestAmount,
                ReservedTime = reservation.ReservedTime,
                Created = reservation.Created,
                Modified = reservation.Modified,
                Note = split[1],
                Phone = split[0],
            };
        }
        public Reservation ToReservation()
        {
            return new Reservation
            {
                Id = Id,
                GuestAmount = GuestAmount,
                Note = AnonymousReservationHelper.MergePhone_Note(Phone, Note),
                Private = Private,
                ReservedTime = ReservedTime,
                Created = Created,
                Modified = Modified,
                TableId = TableId
            };
        }
    }
}
