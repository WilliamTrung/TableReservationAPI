using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.ReservationModels
{
    public class ModifiedReservationModel
    {
        public int Id { get; }
        [Required]
        [EmailAddress]
        public string Email { get; } = null!;

        //For customer only
        [Range(1, 12)]
        public int Seat { get; set; }
        public bool Private { get; set; }
        [DesiredDateValidator]
        public DateOnly DesiredDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);        
        [Required]
        [TimeValidator]
        public TimeOnly DesiredTime { get; set; }
        public string? Note { get; set; }
        
        //For reception only
        public int? AssignedTableId { get; set; }
        //system decide
        public string Status { get; set; } = null!;

    }
}
