using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.GestionEtiquetas.Entities;

namespace Goikoa.Domain.GestionEtiquetas.Interfaces
{
    public interface IServiceCamposWSBartender
    {
        Task<List<string>> GetCamposFaltantesAsync();
        Task<List<string>> RegistrarCamposAsync(List<string> nombres);
        Task<PaginatedResult<string>> GetTodosCamposPaginadoAsync(int page, int pageSize);
    }
}
