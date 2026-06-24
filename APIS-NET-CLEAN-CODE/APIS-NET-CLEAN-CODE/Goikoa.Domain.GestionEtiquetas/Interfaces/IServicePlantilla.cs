using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;

namespace Goikoa.Domain.GestionEtiquetas.Interfaces
{
    public interface IServicePlantilla
    {
        Task AddPlantillaAsync(PlantillaDTO plantillaDto);
        Task<PaginatedResult<PlantillaDTO>> GetPlantillasList(int pageNumber, int pageSize, string filtro);
        Task UpdatePlantillaAsync(PlantillaDTO plantillaDto);
        Task<Plantilla> DeletePlantillaAsync(int id);
    }
}
