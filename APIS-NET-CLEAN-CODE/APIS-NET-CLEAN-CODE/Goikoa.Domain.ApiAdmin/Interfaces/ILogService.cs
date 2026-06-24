using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Entities;

namespace Goikoa.Domain.ApiAdmin.Interfaces
{
    public interface ILogService
    {
        Task<PaginatedResult<LogDTO>> GetAllLogsAsync(int pPageNumber, int pPageSize);
        Task<PaginatedResult<LogDTO>> GetFilteredLogsAsync(DateTime pStartingDate, string pLevel, int pPageNumber, int pPageSize);
    }
}
