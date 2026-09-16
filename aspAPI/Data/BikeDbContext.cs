using aspAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace aspAPI.Data
{
    public class BikeDbContext : DbContext
    {
        private readonly DbContextOptions _options;
        public BikeDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }
        public DbSet<StationInformationDto> StationInformation => Set<StationInformationDto>();
        public DbSet<VehicleTypesDto> VehicleTypes => Set<VehicleTypesDto>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StationInformationDto>()
                .HasKey(a => a.station_id);
            modelBuilder.Entity<VehicleTypesDto>()
                .HasKey(a => a.vehicle_type_id);
        }
    }
}
