using ApplicationService.Models.UserModels;
using ApplicationService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ChangePhoneNumber(UpdatePhoneModel phoneModel, AuthorizedModel requester)
        {
            var find = await _unitOfWork.UserRepository.Get(filter: u => u.Email == requester.Email);
            var user = find.FirstOrDefault();
            if(user == null)
            {
                throw new KeyNotFoundException();
            }
            user.Phone = phoneModel.Phone;
            await _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Commit();
        }
    }
}
