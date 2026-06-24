using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.ApiAdmin.DAL.Context;
using Goikoa.Domain.ApiAdmin.DAL.Models;
using Goikoa.Domain.ApiAdmin.Entities;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Goikoa.Domain.ApiAdmin.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApiAdminContext _context;
        private readonly IMapper _mapper;

        public RefreshTokenRepository(ApiAdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RefreshTokenEntity> GetByTokenAsync(string pToken)
        {
            return _mapper.Map<RefreshTokenEntity>(await _context.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.Token == pToken));
        }

        public async Task AddAsync(RefreshTokenEntity pRefreshToken)
        {
            await _context.RefreshTokens.AddAsync(_mapper.Map<RefreshToken>(pRefreshToken));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshTokenEntity pRefreshToken)
        {
            _context.RefreshTokens.Update(_mapper.Map<RefreshToken>(pRefreshToken));
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefreshTokenEntity>> GetActiveTokensForUserAsync(int pUserId)
        {
            var lLstRefreshTokenActive= await _context.RefreshTokens.AsNoTracking()
                .Where(rt => rt.UserId == pUserId &&
                       rt.Expires > DateTime.UtcNow &&
                       rt.Revoked == null)
                .ToListAsync();
            return _mapper.Map<List<RefreshTokenEntity>>(lLstRefreshTokenActive);
        }
    }
}
