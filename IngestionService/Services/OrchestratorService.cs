using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IngestionService.Services
{
    public class OrchestratorService
    {
        private readonly StationInformationService _stationInformationService;
        private readonly StationStatusService _stationStatusService;
        private readonly VehicleTypeService _vehicleTypeService;
        public OrchestratorService(
            StationInformationService stationInformationService,
            StationStatusService stationStatusService,
            VehicleTypeService vehicleTypeService
        )
        {
            _stationInformationService = stationInformationService;
            _stationStatusService = stationStatusService;
            _vehicleTypeService = vehicleTypeService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var runStationInfo = RunStationInfo(cancellationToken);
            var runVehicleTypes = RunVehicleTypes(cancellationToken);
            var runStationStatus = RunStationStatus(cancellationToken);

            await Task.WhenAll(
                runStationInfo,
                runVehicleTypes,
                runStationStatus
            );
            
        }

        public async Task RunStationInfo(CancellationToken cancellationToken)
        {
            await _stationInformationService.GetAndPublishStationInfo(cancellationToken);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await _stationInformationService.GetAndPublishStationInfo(cancellationToken);
            }
        }

        public async Task RunVehicleTypes(CancellationToken cancellationToken)
        {
            await _vehicleTypeService.GetAndPublishVehicleType(cancellationToken);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await _vehicleTypeService.GetAndPublishVehicleType(cancellationToken);
            }
        }
        
        public async Task RunStationStatus(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await _stationStatusService.GetAndPublishStationStatus(cancellationToken);
            }
        }
    }
}