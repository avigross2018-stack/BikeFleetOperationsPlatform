using System.Text.Json;
using Confluent.Kafka;
using ProcessingService.Models;
using ProcessingService.Repos;

namespace ProcessingService.Services
{
    public class VehicleTypesHandler
    {
        private readonly VehicleTypesRepo _repo;
        public VehicleTypesHandler(
            VehicleTypesRepo repo
        )
        {
            _repo = repo;
        }

        public async Task RunHandler(
            CancellationToken cancellationToken,
            string message
        )
        {
            var modelObj = JsonSerializer.Deserialize<VehicleTypeDto>(message);

            if(modelObj is null)
            {
                return;
            }

            var existInDb = await _repo.GetByVehicleId(modelObj.VehicleTypeId);

            if(existInDb is null)
            {
                await _repo.SaveNewData(modelObj);
                return;
            }

            await _repo.UpdateExist(modelObj, modelObj.VehicleTypeId);
            
        }
    }
}