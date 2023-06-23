using ApplicationCore.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.UserModels
{
    public class UpdateUserModel
    {
        [Required]
        public string Email { get; set; } = null!;
        public IEnum.Role Role { get; set; }
        public string? Phone { get; set; }
    }
}
