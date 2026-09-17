using System.Text.Json;
using Confluent.Kafka;
using ProcessingService.Models;
using ProcessingService.Repos;

namespace ProcessingService.Services
{
    public class StationInfoHandler
    {
        private readonly StationInfoRepo _repo;
        public StationInfoHandler(
            StationInfoRepo repo
        )
        {
            _repo = repo;
        }

        public async Task RunHandler(
            CancellationToken cancellationToken,
            string message
            )
        {

            var modelObj = JsonSerializer.Deserialize<StationInfoDto>(message);

            if(modelObj is null)
            {
                return;
            }

            var existInDb = await _repo.GetByStationId(modelObj.StationId);

            //if obj not exist in Db
            if(existInDb is null)
            {
                await _repo.SaveNewData(modelObj);
                return;
            }

            await _repo.UpdateExist(modelObj, modelObj.StationId);
            
        }
    }
}