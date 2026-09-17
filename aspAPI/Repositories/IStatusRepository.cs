using aspAPI.DTOs;
using aspAPI.Models;

namespace aspAPI.Repositories
{
    public interface IStatusRepository
    {
        Task<StationStatusDto> GetCurrentStationStatusAsync(string id);
        Task<List<StationsDto>> GetByFilter(int? minAvailableBikes, int? isRenting, int? isReturning);
        Task<StationsDto?> GetById(string id);
        Task<List<HistoryDto>> GetStationHistory(DateTime? from, DateTime? to, int? limit);
    }
}

