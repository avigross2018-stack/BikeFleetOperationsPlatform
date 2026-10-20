using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.StationIfo
{
    public class StationInfoDto
    {
        [JsonPropertyName("station_id")]
        public string StationId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("short_name")]
        public string? ShortName { get; set; }

        [JsonPropertyName("lon")]
        public double? Longitude { get; set; }

        [JsonPropertyName("lat")]
        public double? Latitude { get; set; }

        [JsonPropertyName("region_id")]
        public string? RegionId { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("rental_uris")]
        public RentalUrisDto? RentalUris { get; set; }
    }
}