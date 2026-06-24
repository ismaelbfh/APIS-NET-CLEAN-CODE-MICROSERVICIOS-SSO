using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.ApiAdmin.DAL.Models;
using Goikoa.Domain.ApiAdmin.Entities;

namespace Goikoa.Domain.ApiAdmin.Mappers
{
    public class MappingUserEntitiesProfile : Profile
    {
        public MappingUserEntitiesProfile()
        {
            CreateMap<UserEntity, Usuario>()
                .ForMember(dest => dest.PkIdUser, opt => opt.MapFrom(src => src.PkIdUser))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.FkRoleId, opt => opt.MapFrom(src => src.FkRoleId))
                .ReverseMap();

            CreateMap<RoleEntity, Role>()
                .ForMember(dest => dest.PkIdRole, opt => opt.MapFrom(src => src.PkIdRole))
                .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.NombreRol))
                .ReverseMap();
        }
    }
}
