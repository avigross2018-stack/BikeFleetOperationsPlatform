using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.StationStatus
{
    public class StationStatusResponse
    {
        [JsonPropertyName("data")]
        public StationStatusData Data { get; set; } = null!;
    }
}