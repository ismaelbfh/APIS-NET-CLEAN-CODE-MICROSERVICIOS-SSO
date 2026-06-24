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
    public class RoleRepository :IRoleRepository
    {
        private readonly ApiAdminContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(ApiAdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public RoleEntity GetRoleByIdAsync(int? pId)
        {
            Role? lRoleMap = _context.Roles.FirstOrDefault(x => x.PkIdRole == pId);
            return _mapper.Map<RoleEntity>(lRoleMap);
        }
    }
}
