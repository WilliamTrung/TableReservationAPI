using ApplicationCore.Entities;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IGoogleService
    {
        GoogleJsonWebSignature.Payload? ValidateToken(string token);
    }
}
