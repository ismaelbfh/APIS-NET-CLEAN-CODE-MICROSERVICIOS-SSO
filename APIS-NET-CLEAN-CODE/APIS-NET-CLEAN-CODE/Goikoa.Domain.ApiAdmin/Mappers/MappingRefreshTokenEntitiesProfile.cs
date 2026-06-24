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
    public class MappingRefreshTokenEntitiesProfile : Profile
    {
        public MappingRefreshTokenEntitiesProfile()
        {
            CreateMap<RefreshTokenEntity, RefreshToken>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.Expires, opt => opt.MapFrom(src => src.Expires))
                .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ReplacedByToken, opt => opt.MapFrom(src => src.ReplacedByToken))
                .ReverseMap();
        }
    }
}
