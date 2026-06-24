using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface IVisionService
    {
        Task<VisionOrdenResumenDTO> IniciarOrdenAsync(VisionIniciarOrdenRequest pRequest);
        Task<VisionLecturaDTO> RegistrarLecturaAsync(VisionRegistrarLecturaRequest pRequest);
        Task<VisionOrdenResumenDTO> GetResumenByOrdenAsync(string pOrdenFabricacion);

        Task<VisionOrdenResumenDTO> FinalizarOrdenAsync(VisionFinalizarOrdenRequest pRequest);
        Task<List<VisionLecturaDTO>> GetLecturasByResumenIdAsync(Guid pVisionOrdenResumenId);
        Task<List<VisionOrdenResumenDTO>> GetResumenesByOrdenAsync(string pOrdenFabricacion);
        Task<VisionOrdenResumenDTO> ActualizarOrdenAsync(VisionActualizarOrdenRequest pRequest);
    }
}
