using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    [Table("Feedback")]
    public class Feedback
    {
        [Range(0, 5)]
        public int? UtilityRating { get; set; }
        [Range(0, 5)]
        public int? ServiceRating { get; set; }
        [Range(0, 5)]
        public int? FacilityRating { get; set; }
        [Range(0, 5)]
        public int? FoodRating { get; set; }
        public string? Comment { get; set; }
        [Required]
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; } = null!;
    }
}
