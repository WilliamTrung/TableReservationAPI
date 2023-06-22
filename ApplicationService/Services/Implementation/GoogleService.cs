using ApplicationCore.Entities;
using ApplicationService.Models;
using ApplicationService.Models.JwtModels;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class GoogleService : IGoogleService
    {
        private readonly OAuthConfiguration _googleConfig;
        public GoogleService(IOptions<OAuthConfiguration> googleConfig)
        {
            _googleConfig = googleConfig.Value;
        }

        public GoogleJsonWebSignature.Payload? ValidateToken(string token)
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();
            validationSettings.Audience = new[] { _googleConfig.ClientId };
            return GoogleJsonWebSignature.ValidateAsync(token, validationSettings).Result;
        }
    }
}
