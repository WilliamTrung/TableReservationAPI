using ApplicationCore.Entities;
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
        public EnumModel.Role Role { get; set; }
        public string? Phone { get; set; }

        public static UpdateUserModel FromUserEntity(User user)
        {
            return new UpdateUserModel
            {
                Email = user.Email,
                Role = Enum.Parse<EnumModel.Role>(user.Role.ToString()),
                Phone = user.Phone,
            };
        }
    }
}
