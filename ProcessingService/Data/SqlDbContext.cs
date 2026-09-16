using Microsoft.EntityFrameworkCore;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingService.Data
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext() { }
        public SqlDbContext(DbContextOptions options) : base(options) { }
        public DbSet<StationInformationDto> information => Set<StationInformationDto>();
        public DbSet<VehicleTypesDto> vehicles => Set<VehicleTypesDto>();
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
