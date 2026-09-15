using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using IngestionService.Models.VehicleType;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IngestionService.Services
{
    public class VehicleTypeService
    {
        private readonly HttpClientService _httpClientService;
        private readonly ProducerService _producerService;
        private readonly ILogger<VehicleTypeService> _logger;

        private readonly string _endpoint;
        private readonly string _topicName;
        public VehicleTypeService(
            HttpClientService httpClientService,
            ProducerService producerService,
            IConfiguration config,
            ILogger<VehicleTypeService> logger
        )
        {
            _httpClientService = httpClientService;
            _producerService = producerService;
            _logger = logger;

            _endpoint = config["VEHICLE_TYPES_ENDPOINT"] ??
                throw new InvalidOperationException("Some endpoint is missing");
            
            _topicName = config["KAFKA_VEHICLE_TYPES_TOPIC"] ??
                throw new InvalidOperationException("Some topic name is missing");
        }

        public async Task GetAndPublishVehicleType(CancellationToken cancellationToken)
        {
            VehicleTypeResponse? response;
            try
            {
                response = await _httpClientService.GetRequest<VehicleTypeResponse>(
                    _endpoint, cancellationToken
                );
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError("Failed to request from Vehicle Type {ex}", ex);
                return;
            }
            
            if(response?.Data?.VehicleTypes is null)
            {
                _logger.LogWarning("Vehicle Type is NULL");
                return;
            }

            foreach (var vehicleType in response.Data.VehicleTypes)
            {
                if (!IsValid(vehicleType))
                {
                    continue;
                }

                await _producerService.SendAsync<VehicleTypeDto>(
                    vehicleType, _topicName, cancellationToken
                );
                _logger.LogInformation("Send Vehicle Type to Kafka successfully");
            }
        }

        public bool IsValid(VehicleTypeDto vehicleType)
        {
            if(vehicleType.VehicleTypeId is null)
            {
                return false;
            }

            if(vehicleType.MaxRangeMeters.HasValue && vehicleType.MaxRangeMeters < 0)
            {
                return false;
            }

            return true;
        }
    }
}