using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Enum;

namespace ApplicationCore.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }        
        public IEnum.Role Role { get; set; } = IEnum.Role.Customer;
        [Required]
        public string Email { get; set; } = null!;
        [Phone]
        public string? Phone { get; set; }

        public bool Lockout = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public int LockoutCount { get; set; }  = 0;        
    }
}
