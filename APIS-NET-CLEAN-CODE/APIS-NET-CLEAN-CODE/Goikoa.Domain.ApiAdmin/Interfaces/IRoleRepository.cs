using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.ApiAdmin.Entities;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface IRoleRepository
    {
        RoleEntity GetRoleByIdAsync(int? pId);
    }
}
