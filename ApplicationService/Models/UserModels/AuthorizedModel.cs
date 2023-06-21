using ApplicationCore.Entities;
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
        public string Role { get; set; } = null!;

        public static AuthorizedModel Converter(User user)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new AuthorizedModel
            {
                Email = user.Email,
                Role = user.Role.Name
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
