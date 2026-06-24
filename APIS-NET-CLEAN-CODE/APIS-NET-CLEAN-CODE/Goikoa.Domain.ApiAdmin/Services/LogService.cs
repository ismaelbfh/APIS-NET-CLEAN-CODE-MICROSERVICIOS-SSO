using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.Domain.ApiAdmin.DAL.Context;
using Goikoa.Domain.ApiAdmin.DAL.Models;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Entities;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Goikoa.Domain.ApiAdmin.Services
{
    public class LogService : ILogService
    {
        private readonly ApiAdminContext _context;
        private readonly IMapper _mapper;

        public LogService(ApiAdminContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<LogDTO>> GetAllLogsAsync(int pPageNumber, int pPageSize)
        {
            var lQuery = _context.Logs.OrderByDescending(l => l.TimeStamp);
            int lTotalRecords = await lQuery.CountAsync();
            List<Log> lItems = await lQuery
                                .Skip((pPageNumber - 1) * pPageSize)
                                .Take(pPageSize)
                                .ToListAsync();

            return new PaginatedResult<LogDTO>
            {
                Items = _mapper.Map<List<LogDTO>>(lItems),
                TotalRows = lTotalRecords,
                PageNumber = pPageNumber,
                PageSize = pPageSize
            };
        }

        public async Task<PaginatedResult<LogDTO>> GetFilteredLogsAsync(DateTime pStartingDate, string pLevel, int pPageNumber, int pPageSize)
        {
            var lQuery = _context.Logs
                .Where(l => l.TimeStamp >= pStartingDate && l.Level == pLevel)
                .OrderByDescending(l => l.TimeStamp);

            int lTotalRecords = await lQuery.CountAsync();
            List<Log> lItems = await lQuery
                                    .Skip((pPageNumber - 1) * pPageSize)
                                    .Take(pPageSize)
                                    .ToListAsync();

            return new PaginatedResult<LogDTO>
            {
                Items = _mapper.Map<List<LogDTO>>(lItems),
                TotalRows = lTotalRecords,
                PageNumber = pPageNumber,
                PageSize = pPageSize
            };
        }

    }
}
