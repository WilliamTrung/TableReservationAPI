using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
    }
}
