using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class ReviewRating
    {
        [ForeignKey(nameof(Rate))]
        public int RateId { get; set; }
        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; }

        public virtual Rate? Rate { get; set; }
        public virtual Review? Review { get; set; }
    }
}
