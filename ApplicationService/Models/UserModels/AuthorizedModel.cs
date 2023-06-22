using ApplicationCore.Entities;
using ApplicationCore.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Models.UserModels
{
    public class AuthorizedModel
    {
        public string Email { get; set; } = null!;
        public IEnum.Role Role { get; set; }
        public string? Phone { get; set; }

        public static AuthorizedModel Converter(User user)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new AuthorizedModel
            {
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
