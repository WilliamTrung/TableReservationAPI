using ApplicationCore.Entities;
using ApplicationService.Models.JwtModels;
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
    public class JwtService : IJwtService
    {
        private readonly JwtConfiguration _jwtConfig;
        public JwtService(IOptions<JwtConfiguration> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var claims = new ClaimsIdentity();
            claims.AddClaim(claim: new Claim("Email", user.Email));
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            claims.AddClaim(claim: new Claim(ClaimTypes.Role, user.Role.Name));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                              //await AddRolesClaim(ref claims);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims.Claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtConfig.AccessTokenExpirationMinutes)),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidAudience = _jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key))
                }, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return claimsPrincipal;
            }
            catch
            {
                return null;
            }
        }
    }
}
