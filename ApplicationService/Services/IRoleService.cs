using ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> GetRoles();
    }
}
