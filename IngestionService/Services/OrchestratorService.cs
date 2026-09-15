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
            await _stationInformationService.GetAndPublishStationInfo(cancellationToken);
            await _stationStatusService.GetAndPublishStationStatus(cancellationToken);
            await _vehicleTypeService.GetAndPublishVehicleType(cancellationToken);
        }
    }
}