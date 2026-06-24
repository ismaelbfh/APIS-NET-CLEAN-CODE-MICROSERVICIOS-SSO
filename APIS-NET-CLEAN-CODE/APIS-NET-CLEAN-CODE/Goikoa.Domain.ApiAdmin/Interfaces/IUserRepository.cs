using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.ApiAdmin.Entities;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserEntity>> GetUsersAllAsync();
        Task<UserEntity> GetUserByIdAsync(int id);
        Task<UserEntity> GetUserByNameAsync(string username);
        Task<UserEntity> AddAsync(UserEntity pUserEntity);
    }
}
