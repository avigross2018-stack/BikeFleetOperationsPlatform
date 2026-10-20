using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcessingService.Db;
using ProcessingService.Models;

namespace ProcessingService.Repos
{
    public class VehicleTypesRepo
    {
        private readonly BikesDbContext _dbContext;
        public VehicleTypesRepo(BikesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VehicleTypeDto?> GetByVehicleId(string VehicleId)
        {
            return await _dbContext.VehicleType
                .FirstOrDefaultAsync(v => v.VehicleTypeId == VehicleId);
        }

        public async Task SaveNewData(VehicleTypeDto vehicle)
        {
            await _dbContext.VehicleType.AddAsync(vehicle);

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateExist(VehicleTypeDto vehicle, string VehicleId)
        {
            var exist = await GetByVehicleId(VehicleId);

            exist!.FormFactor = vehicle.FormFactor;
            exist.PropulsionType = vehicle.PropulsionType;
            exist.MaxRangeMeters = vehicle.MaxRangeMeters;

            await _dbContext.SaveChangesAsync();
        }
    }
}