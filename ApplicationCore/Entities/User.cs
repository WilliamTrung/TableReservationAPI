using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        public bool Lockout = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public int LockoutCount { get; set; }  = 0;

        public virtual Role? Role { get; set; }
    }
}
