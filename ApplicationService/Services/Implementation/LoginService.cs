using ApplicationCore.Entities;
using ApplicationService.Models;
using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using Google.Apis.Auth;
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
        private readonly IGoogleService _googleService;
        private readonly Domain _domain;
        public LoginService(IUnitOfWork unitOfWork, IGoogleService googleService, IOptions<Domain> domain)
        {
            _unitOfWork = unitOfWork;
            _googleService = googleService;
            _domain = domain.Value;
        }

        /// <summary>
        /// Get AuthorizedModel from authorization token
        /// <para>Throw Exception: Not a bearer JWT token or No token</para>
        /// <para>Throw InvalidJwtException: Invalid token or payload</para>
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidJwtException"></exception>
        public async Task<AuthorizedModel> ValidateLoginAsync(string? authHeader)
        {
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var payload = _googleService.ValidateToken(token);
                        if(payload != null)
                        {
                            var email = payload.Email;
                            var user = await ValidateEmailAsync(email);
                            if(user == null)
                            {
                                user = await Register(email);
                            }
                            return AuthorizedModel.Converter(user);
                        } else
                        {
                            throw new InvalidJwtException("Unable to load payload!");
                        }                        
                    }
                    catch
                    {
                        throw new InvalidJwtException("Invalid token");
                    }
                }
            }
            throw new Exception("Unauthorized");
        }
        private async Task<User?> ValidateEmailAsync(string email)
        {
            var find = await _unitOfWork.UserRepository.Get(filter: u => u.Email == email);
            return find.FirstOrDefault();
        }
        public async Task<User> Register(string email)
        {
            var task_find = _unitOfWork.UserRepository.Get(filter: u => u.Email == email);
            var found = task_find.Result.FirstOrDefault();
            if(found == null)
            {
                var user = new User
                {
                    Email = email
                };
                if (email.Substring(email.IndexOf('@') + 1) == _domain.Name){
                    //is staff
                    user.Role = ApplicationCore.Enum.IEnum.Role.Reception;
                } else
                {
                    //is customer
                    user.Role = ApplicationCore.Enum.IEnum.Role.Customer;
                }
                await _unitOfWork.UserRepository.Create(user);
                _unitOfWork.Commit();
                return user;
            } else
            {
                throw new Exception("Already registered!");
            }
        }
    }
}
