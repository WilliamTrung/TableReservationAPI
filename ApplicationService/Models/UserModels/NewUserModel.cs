using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.UserModels
{
    public class NewUserModel
    {
        [Required]
        public string Email { get; set; } = null!;
        [Phone]
        public string Phone { get; set; } = null!;
    }
}
