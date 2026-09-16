using aspAPI.DTOs;
using aspAPI.Models;

namespace aspAPI.Repositories
{
    public interface IStatusRepository
    {
        Task<StationStatusDto> GetCurrentStationStatusAsync(string id);
        Task<List<StationsDto>> GetByFilter(int? minAvailableBikes, int? isRenting, int? isReturning);
    }
}

