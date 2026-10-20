using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcessingService.Models;

namespace ProcessingService.Db
{
    public class BikesDbContext : DbContext
    {
        public BikesDbContext(DbContextOptions<BikesDbContext> options)
                :base(options){}

        public DbSet<StationInfoDto> StationInfo{ get; set; } = null!;
        public DbSet<VehicleTypeDto> VehicleType { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StationInfoDto>()
                .HasKey(k => k.StationId);

            modelBuilder.Entity<VehicleTypeDto>()
                .HasKey(k => k.VehicleTypeId);
        }
    }
}