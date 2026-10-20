using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;
using MongoDB.Driver.Search;
using ProcessingService.Models;
using StackExchange.Redis;

namespace ProcessingService.Services
{
    public class RedisService
    {
        private readonly StackExchange.Redis.IDatabase _database;
        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<T?> GetLastUpload<T>(string stationId)
        {
            var key = $"ID:{stationId}";

            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T?>(value!);
        }

        public async Task SetLastUpload(StationStatusDto station)
        {
            var key = $"ID:{station.StationId}";

            var json = JsonSerializer.Serialize(station);

            await _database.StringSetAsync(key, json);
        }
    }
}