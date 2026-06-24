using System.Text.Json;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Entities;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_NAV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        // GET api/logs?pageNumber=1&pageSize=50
        [HttpGet]
        public async Task<IActionResult> GetAllLogs(int pageNumber = 1, int pageSize = 50)
        {
            PaginatedResult<LogDTO> result = await _logService.GetAllLogsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // GET api/logs/filter?startingDate=2025-02-10&level=Error&pageNumber=1&pageSize=50
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredLogs(
            DateTime startingDate,
            string level,
            int pageNumber = 1,
            int pageSize = 50)
        {
            PaginatedResult<LogDTO> result = await _logService.GetFilteredLogsAsync(startingDate, level, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
