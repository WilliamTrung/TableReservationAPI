using ApplicationCore.Entities;
using ApplicationService.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Get AuthorizedModel from authorization token
        /// <para>Throw Exception: Not a bearer JWT token or No token</para>
        /// <para>Throw InvalidJwtException: Invalid token or payload</para>
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidJwtException"></exception>
        Task<AuthorizedModel> ValidateLoginAsync(string? authHeader);
        Task<User> Register(string email);
    }
}
