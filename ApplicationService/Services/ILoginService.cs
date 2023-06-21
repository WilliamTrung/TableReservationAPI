using ApplicationService.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface ILoginService
    {
        AuthorizedModel ValidateLogin(string? authHeader);
        /// <summary>
        /// Get the user credentials -- if not exist --> create new credentials and return it
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<string> GetAccessToken(string email);
        Task Register(string email);
    }
}
