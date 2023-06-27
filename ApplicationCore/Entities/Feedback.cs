using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }  
        public string? Comment { get; set; }
        [Required]
        [ForeignKey(nameof(Reservation))]
        public int ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; } = null!;
    }
}
