using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace ProcessingService.Services
{
    public class OrchestratorService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IConfiguration _config;
        private readonly StationInfoHandler _stationInfo;
        private readonly StationStatusHandler _stationStatus;
        private readonly VehicleTypesHandler _vehicleTypes;
        public OrchestratorService(
            StationInfoHandler stationInfo,
            StationStatusHandler stationStatus,
            VehicleTypesHandler vehicleTypes,
            IConsumer<Ignore, string> consumer,
            IConfiguration config
        )
        {
            _stationInfo = stationInfo;
            _stationStatus = stationStatus;
            _vehicleTypes = vehicleTypes;
            _consumer = consumer;
            _config = config;
        }

        public async Task RunAsync(
            CancellationToken cancellationToken,
            string[] topicNames
            )
        {
            _consumer.Subscribe(topicNames);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumer.Consume(TimeSpan.FromSeconds(1));

                        if (result is null || result.Message?.Value is null)
                        {
                            continue;
                        }

                        switch (result.Topic)
                        {
                            case var topic when topic == _config["KAFKA_STATION_INFO_TOPIC"]:
                                await _stationInfo.RunHandler(
                                    cancellationToken, result.Message.Value
                                );
                                break;
                            case var topic when topic == _config["KAFKA_STATION_STATUS_TOPIC"]:
                                await _stationStatus.RunHandler(
                                    cancellationToken, result.Message.Value
                                );
                                break;
                            case var topic when topic == _config["KAFKA_VEHICLE_TYPES_TOPIC"]:
                                await _vehicleTypes.RunHandler(
                                    cancellationToken, result.Message.Value
                                );
                                break;
                            default:
                                continue;
                        }

                        _consumer.Commit(result);
                    }
                    catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
                    {
                        System.Console.WriteLine($"kafka topic is not available {ex.Error.Reason}");
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    }

                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {

            }
            finally
            {
                _consumer.Close();
            }
        }
    }
}