using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.ApiAdmin.Entities;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenEntity> GetByTokenAsync(string pToken);
        Task AddAsync(RefreshTokenEntity pRefreshToken);
        Task UpdateAsync(RefreshTokenEntity pRefreshToken);
        Task<List<RefreshTokenEntity>> GetActiveTokensForUserAsync(int pUserId);

    }
}
