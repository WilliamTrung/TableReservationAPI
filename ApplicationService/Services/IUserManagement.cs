using ApplicationCore.Entities;
using ApplicationService.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IUserManagement
    {
        public Task Add(NewUserModel newUser);
        public Task Update(UpdateUserModel updateUser);
        public Task Lockout(User user);
    }
}
