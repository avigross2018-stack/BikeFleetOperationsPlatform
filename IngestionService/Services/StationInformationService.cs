using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IngestionService.Models;
using IngestionService.Models.StationIfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IngestionService.Services
{
    public class StationInformationService
    {
        private readonly HttpClientService _httpClientService;
        private readonly ProducerService _producerService;
        private readonly ILogger<StationInformationService> _logger;

        private readonly string _topicName;
        private readonly string _endpoint;

        public StationInformationService(
            HttpClientService httpClientService,
            ProducerService producerService,
            IConfiguration config,
            ILogger<StationInformationService> logger
        )
        {
            _httpClientService = httpClientService;
            _producerService = producerService;
            _logger = logger;

            _endpoint = config["STATION_INFO_ENDPOINT"] ??
                throw new InvalidOperationException("Some endpoint is missing");

            _topicName = config["KAFKA_STATION_INFO_TOPIC"] ??
                throw new InvalidOperationException("Some topic name is missing");
        }

        public async Task GetAndPublishStationInfo(CancellationToken cancellationToken)
        {
            StationInfoResponse? response;

            try
            {
                response = await _httpClientService.GetRequest<StationInfoResponse>(
                    _endpoint, cancellationToken
                );
            }
            catch(HttpRequestException ex)
            {
                _logger.LogError("Failed to request from Station Info {ex}", ex);
                return;
            }
            if(response?.Data?.Stations is null)
            {
                _logger.LogWarning("Station Info is NULL");
                return;
            }

            foreach (var station in response.Data.Stations)
            {
                if (!IsValid(station))
                {
                    _logger.LogInformation("Station Info is not Valid");
                    continue;
                }

                var kafkaModel = new StationInfoKafka
                {
                    StationId = station.StationId,
                    Name = station.Name,
                    ShortName = station.ShortName,
                    Longitude = station.Longitude,
                    Latitude = station.Latitude,
                    RegionId = station.RegionId,
                    Capacity = station.Capacity
                };
                await _producerService.SendAsync<StationInfoKafka>(
                    kafkaModel, _topicName, cancellationToken
                );
                _logger.LogInformation("Send Station Info to Kafka successfully");
            }
        }

        public bool IsValid(StationInfoDto station)
        {
            if (string.IsNullOrWhiteSpace(station.StationId))
            {
                return false;
            }

            if(station.Latitude is null ||
                station.Latitude < -90 ||
                station.Latitude > 90)
            {
                return false;
            }

            if(station.Longitude is null ||
                station.Longitude < -180 ||
                station.Longitude > 180)
            {
                return false;
            }

            if(station.Capacity < 0)
            {
                return false;
            }

            return true;
        }
    }
}