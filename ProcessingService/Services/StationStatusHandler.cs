using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using ProcessingService.Models;

namespace ProcessingService.Services
{
    public class StationStatusHandler
    {
        private readonly RedisService _redis;
        private readonly MongoService _mongo;
        public StationStatusHandler(
            RedisService redis,
            MongoService mongo
            )
        {
            _mongo = mongo;
            _redis = redis;
        }

        public async Task RunHandler(
            CancellationToken cancellationToken,
            string message
            )
        {
            var modelObj = JsonSerializer.Deserialize<StationStatusDto>(message);

            var redisResult = await _redis.GetLastUpload<StationStatusDto>(modelObj!.StationId);

            //check if the obj not in redis
            if(redisResult is null)
            {
                await _mongo.SaveAsync(modelObj, cancellationToken);

                await _redis.SetLastUpload(modelObj);

                return;
            }

            //check if new data came
            if(redisResult.NumBikesAvailable != modelObj.NumBikesAvailable ||
                redisResult.NumBikesDisabled != modelObj.NumBikesDisabled ||
                redisResult.NumDocksAvailable != modelObj.NumDocksAvailable ||
                redisResult.NumDocksDisabled != modelObj.NumDocksDisabled ||
                redisResult.IsInstalled != modelObj.IsInstalled ||
                redisResult.IsRenting != modelObj.IsRenting ||
                redisResult.IsReturning != modelObj.IsReturning ||
                redisResult.NumEbikesAvailable != modelObj.NumEbikesAvailable
                )
            {
                await _mongo.SaveAsync(modelObj, cancellationToken);

                await _redis.SetLastUpload(modelObj);

                return;
            }
            
        }
    }
}