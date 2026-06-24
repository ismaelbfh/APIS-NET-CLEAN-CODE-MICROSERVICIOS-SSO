using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.Navision.Producccion.DAL.Models;
using Goikoa.Domain.Navision.Producccion.DTOs.Responses;

namespace Goikoa.Domain.Navision.Producccion.Mappers
{
    public class MappingOrdenesProfile : Profile
    {
        public MappingOrdenesProfile()
        {
            CreateMap<GKInfoOrdenCamaraDTO, GoikoaOrdenProducciónFabricación>()                
                .ForMember(dest => dest.No, opt => opt.MapFrom(src => src.OPFabricacion))
                .ForMember(dest => dest.ItemNo, opt => opt.MapFrom(src => src.CodigoProducto))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.StartingDate, opt => opt.MapFrom(src => src.Fecha))
                .ForMember(dest => dest.LineNo, opt => opt.MapFrom(src => src.Linea))
                .ForMember(dest => dest.CodigoEmbalaje, opt => opt.MapFrom(src => src.CodigoEmbalaje))
                .ForMember(dest => dest.CantidadAProducirEmbalajes, opt => opt.MapFrom(src => src.CantidadAProducirEmbalajes))
                .ReverseMap();

            CreateMap<GKProductoDTO, GoikoaItemCrossReference>()
                .ForMember(dest => dest.ItemNo, opt => opt.MapFrom(src => src.CodigoProducto))
                .ForMember(dest => dest.CrossReferenceTypeNo, opt => opt.MapFrom(src => src.TipoCodigoBarras))
                .ForMember(dest => dest.CrossReferenceNo, opt => opt.MapFrom(src => src.CodificacionCodigoBarras))
                .ReverseMap();
        }
    }
}
