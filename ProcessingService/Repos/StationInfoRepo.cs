using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProcessingService.Db;
using ProcessingService.Models;

namespace ProcessingService.Repos
{
    public class StationInfoRepo
    {
        private readonly BikesDbContext _dbContext;
        public StationInfoRepo(BikesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StationInfoDto?> GetByStationId(string stationId)
        {
            return await _dbContext.StationInfo
                .FirstOrDefaultAsync(s => s.StationId == stationId);
        }

        public async Task SaveNewData(StationInfoDto station)
        {
            await _dbContext.StationInfo
                .AddAsync(station);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateExist(StationInfoDto station, string stationId)
        {
            var exist = await GetByStationId(stationId);

            exist!.Name = station.Name;
            exist!.ShortName = station.ShortName;
            exist!.Longitude = station.Longitude;
            exist!.Latitude = station.Latitude;
            exist!.RegionId = station.RegionId;
            exist!.Capacity = station.Capacity;

            await _dbContext.SaveChangesAsync();
        }
    }
}