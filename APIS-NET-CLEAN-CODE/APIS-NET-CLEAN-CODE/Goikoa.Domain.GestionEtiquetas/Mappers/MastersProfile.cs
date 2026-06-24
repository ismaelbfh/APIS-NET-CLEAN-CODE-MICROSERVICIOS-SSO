using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Requests;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;

namespace Goikoa.Domain.GestionEtiquetas.Mappers
{
    public class MastersProfile : Profile
    {
        public MastersProfile()
        {
            CreateMap<Plantilla, PlantillaDTO>()
                .ReverseMap();

            CreateMap<Etiqueta, EtiquetaDTO>()
                .ReverseMap();

            CreateMap<EtiquetaCodigosBarra, EtiquetaCodigoBarraDTO>()
                .ReverseMap();

            CreateMap<EtiquetaCamposValores, EtiquetaCampoValorDTO>()
                .ReverseMap();

            CreateMap<PlantillaCampos, PlantillaCampoDTO>()
                .ForMember(dest => dest.PK_IdPlantillaCampo, opt => opt.MapFrom(src => src.PK_IdPlantillaCampo))
                .ForMember(dest => dest.FK_IdPlantilla, opt => opt.MapFrom(src => src.FK_IdPlantilla))
                .ForMember(dest => dest.FK_IdCampo, opt => opt.MapFrom(src => src.FK_IdCampo))
                .ForMember(dest => dest.NombreCampoBartender, opt => opt.MapFrom(src => src.NombreCampoBartender))
                .ForMember(dest => dest.Orden, opt => opt.MapFrom(src => src.Orden))
                .ForMember(dest => dest.EsObligatorio, opt => opt.MapFrom(src => src.EsObligatorio))
                .ReverseMap();

            CreateMap<EtiquetaLabels, EtiquetaLabelsDTO>()
                .ForMember(dest => dest.PK_IdEtiquetaLabel, opt => opt.MapFrom(src => src.PK_IdEtiquetaLabel))
                .ForMember(dest => dest.FK_IdEtiqueta, opt => opt.MapFrom(src => src.FK_IdEtiqueta))
                .ForMember(dest => dest.FK_IdTipoLabel, opt => opt.MapFrom(src => src.FK_IdTipoLabel))
                .ForMember(dest => dest.FK_IdIdioma, opt => opt.MapFrom(src => src.FK_IdIdioma))
                .ForMember(dest => dest.FK_IdLabel, opt => opt.MapFrom(src => src.FK_IdLabel))
                .ForMember(dest => dest.Orden, opt => opt.MapFrom(src => src.Orden))
                .ReverseMap();

            CreateMap<HistoricoImpresion, HistoricoImpresionDTO>().ReverseMap();

            CreateMap<CampoDTO, Campo>()
                .ForMember(dest => dest.PK_IdCampo, opt => opt.MapFrom(src => src.PkIdCampo))
                .ForMember(dest => dest.NombreCampo, opt => opt.MapFrom(src => src.NombreCampo))
                .ForMember(dest => dest.FK_IdTipoCampo, opt => opt.MapFrom(src => src.IdTipoCampo))
                .ForPath(dest => dest.FK_IdTipoCampoNavigation.Descripcion, opt => opt.MapFrom(src => src.DescripcionTipoCampo))
                .ForMember(dest => dest.FK_IdCampoNavision, opt => opt.MapFrom(src => src.IdCampoNavision))
                .ForPath(dest => dest.FK_IdCampoNavisionNavigation.Descripcion, opt => opt.MapFrom(src => src.DescripcionCampoNavision))
                .ReverseMap();

            CreateMap<CampoDTORequest, Campo>()
                    .ForMember(dest => dest.PK_IdCampo, opt => opt.MapFrom(src => src.PkIdCampo))
                    .ForMember(dest => dest.NombreCampo, opt => opt.MapFrom(src => src.NombreCampo))
                    .ForMember(dest => dest.FK_IdTipoCampo, opt => opt.MapFrom(src => src.IdTipoCampo))
                    .ForMember(dest => dest.FK_IdCampoNavision, opt => opt.MapFrom(src => src.IdCampoNavision))
                    .ReverseMap();

            CreateMap<CamposNavision, CamposNavisionDTO>()
            .ForMember(dest => dest.PkIdCampoNavision, opt => opt.MapFrom(src => src.PK_IdCampoNavision))
            .ReverseMap();

            CreateMap<MasterDataDTO, CamposNavision>()
            .ForMember(dest => dest.PK_IdCampoNavision, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

            CreateMap<MasterDataDTO, TiposCampo>()
                .ForMember(dest => dest.PK_IdTipoCampo, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, TiposLabels>()
                .ForMember(dest => dest.PK_IdTipoLabel, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, IdiomasLabels>()
                .ForMember(dest => dest.PK_IdIdioma, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<LabelDTORequest, LabelsMaster>()
                    .ForMember(dest => dest.PK_IdLabel, opt => opt.MapFrom(src => src.PKIdLabel))
                    .ForMember(dest => dest.DescripcionLabel, opt => opt.MapFrom(src => src.DescripcionLabel))
                    .ForMember(dest => dest.FK_IdTipoLabel, opt => opt.MapFrom(src => src.FKIdTipoLabel))
                    .ForMember(dest => dest.FK_IdIdioma, opt => opt.MapFrom(src => src.FKIdIdioma))
                    .ReverseMap();
            CreateMap<MasterDataDTO, Ubicaciones>()
                .ForMember(dest => dest.PK_IdUbicacion, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<LabelDTO, LabelsMaster>()
                .ForMember(dest => dest.PK_IdLabel, opt => opt.MapFrom(src => src.PkIdLabel))
                .ForMember(dest => dest.DescripcionLabel, opt => opt.MapFrom(src => src.DescripcionLabel))
                .ForMember(dest => dest.FK_IdTipoLabel, opt => opt.MapFrom(src => src.IdTipoLabel))
                .ForPath(dest => dest.FK_IdTipoLabelNavigation.Descripcion, opt => opt.MapFrom(src => src.DescripcionTipoLabel))
                .ForMember(dest => dest.FK_IdIdioma, opt => opt.MapFrom(src => src.IdIdiomaLabel))
                .ForPath(dest => dest.FK_IdIdiomaNavigation.Descripcion, opt => opt.MapFrom(src => src.DescripcionIdiomaLabel))
                .ReverseMap();

            CreateMap<MasterDataDTO, TiposEtiqueta>()
                .ForMember(dest => dest.PK_IdTipoEtiqueta, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, TiposCodigoBarra>()
                .ForMember(dest => dest.PK_IdTipoCodigoBarra, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, TiposEnvasado>()
                .ForMember(dest => dest.PK_IdEnvasado, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, TiposConservacion>()
                .ForMember(dest => dest.PK_IdConservacion, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, Secciones>()
                .ForMember(dest => dest.PK_IdSeccion, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ReverseMap();

            CreateMap<MasterDataDTO, Cliente>()
               .ForMember(dest => dest.PK_IdCliente, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
               .ReverseMap();
        }
    }
}
