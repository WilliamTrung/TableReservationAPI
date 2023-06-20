using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.IType;

namespace ApplicationCore.Entities
{
    public class Reservation : IAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(Table))]
        public int? TableId { get; set; }
        [ForeignKey(nameof(ReservationStatus))]
        public int StatusId { get; set; }
        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; }
        [Range(1, int.MaxValue)]
        public int GuestAmount { get; set; }  
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        [Required]
        public DateTimeOffset ReservedTime { get; set; }
        public bool Private { get; set; } = false;
        public string? Note { get; set; }

        public virtual User? User { get; set; }
        public virtual Table? Table { get; set; }
        public virtual ReservationStatus? Status { get; set; }
        public virtual Review? Review { get; set; }
        
    }
}
