
using ApplicationService.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IUserService
    {
         Task ChangePhoneNumber(UpdatePhoneModel phoneModel, AuthorizedModel requester);        
    }
}
