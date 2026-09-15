using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IngestionService.Models.StationStatus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IngestionService.Services
{
    public class StationStatusService
    {
        private readonly HttpClientService _httpClientService;
        private readonly ProducerService _producerService;
        private readonly ILogger<StationStatusService> _logger;

        private readonly string _endpoint;
        private readonly string _topicName;

        public StationStatusService(
            HttpClientService httpClientService,
            ProducerService producerService,
            IConfiguration config,
            ILogger<StationStatusService> logger
        )
        {
            _httpClientService = httpClientService;
            _producerService = producerService;
            _logger = logger;

            _endpoint = config["STATION_STATUS_ENDPOINT"] ??
                throw new InvalidOperationException("Some endpoint is missing");
            
            _topicName = config["KAFKA_STATION_STATUS_TOPIC"] ??
                throw new InvalidOperationException("Some topic name is missing");
        }

        public async Task GetAndPublishStationStatus(CancellationToken cancellationToken)
        {
            StationStatusResponse? response;
            try
            {
                response = await _httpClientService.GetRequest<StationStatusResponse>(
                    _endpoint, cancellationToken
                );
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError("Failed to request from Station Status {ex}", ex);
                return;
            }

            if(response?.Data?.Stations is null)
            {
                _logger.LogWarning("Station Status is NULL");
                return;
            }

            foreach (var station in response.Data.Stations)
            {
                if (!IsValid(station))
                {
                    _logger.LogInformation("Station Status is not Valid");
                    continue;
                }

                await _producerService.SendAsync<StationStatusDto>(
                    station, _topicName, cancellationToken
                );
                _logger.LogInformation("Send Station Status to Kafka successfully");
            }
        }

        public bool IsValid(StationStatusDto station)
        {
            if (string.IsNullOrWhiteSpace(station.StationId))
            {
                return false;
            }

            if(station.NumBikesAvailable < 0 ||
                station.NumBikesDisabled < 0 ||
                station.NumDocksAvailable < 0 ||
                station.NumDocksDisabled < 0 ||
                station.NumEbikesAvailable < 0)
            {
                return false;
            }
            return true;
        }
    }
}