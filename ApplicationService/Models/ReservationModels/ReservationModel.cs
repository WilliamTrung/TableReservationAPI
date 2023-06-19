using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.ReservationModels
{
    public class ReservationModel
    {
        public string? Email { get; set; }
        [ForeignKey(nameof(Table))]
        public int TableId { get; set; }
        [ForeignKey(nameof(ReservationStatus))]
        public int StatusId { get; set; }
        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; }
        [Range(1, int.MaxValue)]
        public int GuestAmount { get; set; }
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
        [Required]
        public DateTimeOffset ReservedTime { get; set; }
        public string? Note { get; set; }
    }
}
