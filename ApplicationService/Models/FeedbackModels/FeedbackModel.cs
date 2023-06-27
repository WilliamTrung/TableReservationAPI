using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.FeedbackModels
{
    public class FeedbackModel
    {
        [Required]
        public int ReservationId { get; set; }
        [Range(0, 5)]
        public double Rating { get; set; }
        public string? Comment { get; set; }
    }
}
