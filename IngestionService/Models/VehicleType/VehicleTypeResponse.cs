using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.VehicleType
{
    public class VehicleTypeResponse
    {
        [JsonPropertyName("data")]
        public VehicleTypeData Data { get; set; } = null!;

        [JsonPropertyName("last_updated")]
        public long LastUpdated { get; set; }
        
        [JsonPropertyName("ttl")]
        public int Ttl { get; set; }
        
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;
    }
}