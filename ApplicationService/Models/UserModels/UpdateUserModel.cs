using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.UserModels
{
    public class UpdateUserModel
    {
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Role { get; set; }
    }
}
