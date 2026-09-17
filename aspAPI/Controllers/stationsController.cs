using aspAPI.DTOs;
using aspAPI.Models;
using aspAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace aspAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class stationsController : ControllerBase
    {
        private readonly IStatusRepository _statusRepository;
        public stationsController(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StationsDto>>> GetStations(int? minAvailableBikes, int? isRenting, int? isReturning)
        {
            var result = await _statusRepository.GetByFilter(minAvailableBikes, isRenting, isReturning);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<StationsDto>> GetById(string id)
        {
            var result = await _statusRepository.GetById(id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("{id}/status")]
        public async Task<ActionResult<StationStatusDto>> GetStatusById(string id)
        {
            var result = await _statusRepository.GetCurrentStationStatusAsync(id);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<HistoryDto>>> GetHistory(DateTime? from, DateTime? to, int? limit)
        {
            var result = await _statusRepository.GetStationHistory(from, to, limit);
            return Ok(result);
        }
    }
}
