using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;
using ApplicationCore.Enum;

namespace ApplicationService.Models.ReservationModels
{
    public class ReservationModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public int Seat { get; set; }
        public bool Private { get; set; }
        public DateOnly Date { get; set; }        
        public TimeOnly Time { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string? Note { get; set; } = null!;
        // for reception only
        public int? AssignedTableId { get; set; }
        //system decide
        public IEnum.ReservationStatus Status { get; set; } 


        public static ReservationModel FromReservation(Reservation reservation)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new ReservationModel
            {
                Email = reservation.User.Email,
                Id = reservation.Id,
                Seat = reservation.GuestAmount,
                Private = reservation.Private,
                Date = DateOnly.FromDateTime(reservation.ReservedTime),
                Time = TimeOnly.FromDateTime(reservation.ReservedTime),
                ModifiedDate = reservation.Modified,
                Note = reservation.Note,
                AssignedTableId = reservation.TableId,
                Status = reservation.Status
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
