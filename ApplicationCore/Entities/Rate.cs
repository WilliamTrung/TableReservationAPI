using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Rate
    {
        public Rate()
        {
            ReviewRatings = new HashSet<ReviewRating>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Detail { get; set; } = null!;

        public virtual ICollection<ReviewRating> ReviewRatings { get; set; }
    }
}
