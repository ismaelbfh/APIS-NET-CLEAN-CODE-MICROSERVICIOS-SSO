using AutoMapper;
using Goikoa.Domain.ApiAdmin.DAL.Models;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.Mappers
{
    public class MappingVisionProfile : Profile
    {
        public MappingVisionProfile()
        {
            CreateMap<VisionIniciarOrdenRequest, VisionOrdenResuman>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaHoraInicio, opt => opt.Ignore())
                .ForMember(dest => dest.FechaHoraUltimaLectura, opt => opt.Ignore())
                .ForMember(dest => dest.TotalLecturas, opt => opt.Ignore())
                .ForMember(dest => dest.TotalOk, opt => opt.Ignore())
                .ForMember(dest => dest.TotalNok, opt => opt.Ignore())
                .ForMember(dest => dest.Activa, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<VisionOrdenResuman, VisionOrdenResumenDTO>().ReverseMap();

            CreateMap<VisionLectura, VisionLecturaDTO>().ReverseMap();
        }
    }
}
