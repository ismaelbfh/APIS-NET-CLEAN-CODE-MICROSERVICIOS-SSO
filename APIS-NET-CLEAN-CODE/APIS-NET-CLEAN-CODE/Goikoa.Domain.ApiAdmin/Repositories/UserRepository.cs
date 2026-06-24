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
    public class UserRepository : IUserRepository
    {
        private readonly ApiAdminContext _context;
        private readonly IMapper _mapper;

        public UserRepository(ApiAdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserEntity> GetUserByIdAsync(int pId)
        {
            Usuario? lUserMap = await _context.Usuarios.Include(x => x.FkRole).FirstOrDefaultAsync(x => x.PkIdUser == pId);
            return _mapper.Map<UserEntity>(lUserMap);
        }

        public async Task<UserEntity> GetUserByNameAsync(string pUsername)
        {
            Usuario? userMap = await _context.Usuarios.Include(x => x.FkRole).FirstOrDefaultAsync(x => x.Username.Equals(pUsername));
            return _mapper.Map<UserEntity>(userMap);
        }

        public async Task<IEnumerable<UserEntity>> GetUsersAllAsync()
        {
            IEnumerable<Usuario> userMap = await _context.Usuarios.Include(x => x.FkRole).ToListAsync();
            return _mapper.Map<IEnumerable<UserEntity>>(userMap);
        }

        public async Task<UserEntity> AddAsync(UserEntity pUserEntity)
        {
            Usuario lUser = _mapper.Map<Usuario>(pUserEntity);
            _context.Usuarios.Add(lUser);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserEntity>(lUser);
        }

    }
}
