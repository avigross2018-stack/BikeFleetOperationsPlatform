using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ProcessingService.Models;

namespace ProcessingService.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<StationStatusDto> _collection;
        public MongoService(IMongoDatabase database, IConfiguration config)
        {
            _collection = database.GetCollection<StationStatusDto>(
                config["MONGO_COLLECTION"]
            );
        }

        public async Task SaveAsync(StationStatusDto station, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(station, cancellationToken:cancellationToken);
        }
    }
}