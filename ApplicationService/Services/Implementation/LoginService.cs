using ApplicationCore.Entities;
using ApplicationService.Models;
using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly Domain _domain;
        public LoginService(IUnitOfWork unitOfWork, IJwtService jwtService, IOptions<Domain> domain)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _domain = domain.Value;

        }
        /// <summary>
        /// Get AuthorizedModel from authorization token
        /// <para>Throw Exception: Not a bearer JWT token or No token</para>
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public AuthorizedModel ValidateLogin(string? authHeader)
        {
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var claims = _jwtService.ValidateToken(token);
                        if (claims == null)
                        {
                            throw new Exception();
                        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        return new AuthorizedModel
                        {
                            Email = claims.FindFirst("Email").Value,
                            Role = claims.FindFirst(ClaimTypes.Role).Value
                        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    catch
                    {
                        throw new Exception("Invalid token");
                    }
                }
            }
            throw new Exception("Unauthorized");
        }

        public async Task<string> GetAccessToken(string email)
        {
            var find = ValidateEmailAsync(email).Result;
            if(find == null)
            {
                await Register(email);
            } 
            find = (await _unitOfWork.UserRepository.Get(filter: u => u.Email == email, includeProperties: "Role")).First();
            return _jwtService.GenerateAccessToken(find);
        }
        private async Task<User?> ValidateEmailAsync(string email)
        {
            var find = await _unitOfWork.UserRepository.Get(filter: u => u.Email == email);
            return find.FirstOrDefault();
        }
        public async Task Register(string email)
        {
            var task_find = _unitOfWork.UserRepository.Get(filter: u => u.Email == email);
            var task_role = _unitOfWork.RoleRepository.Get();
            Task.WaitAll(task_find, task_role);
            var found = task_find.Result.FirstOrDefault();
            if(found == null)
            {
                var user = new User
                {
                    Email = email
                };
                if (email.Substring(email.IndexOf('@')) == _domain.Name){
                    //is staff
                    user.RoleId = task_role.Result.First(r => r.Name == "Reception").Id;
                } else
                {
                    //is customer
                    user.RoleId = task_role.Result.First(r => r.Name == "Customer").Id;
                }
                await _unitOfWork.UserRepository.Create(user);
                _unitOfWork.Commit();

            } else
            {
                throw new Exception("Already registered!");
            }
        }
    }
}
