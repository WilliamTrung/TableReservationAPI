using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.ReservationModels
{
    public class AssignTableReservationModel
    {
        [Required]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool Private { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
    }
}
