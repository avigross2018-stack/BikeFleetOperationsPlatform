using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.VehicleType
{
    public class VehicleTypeData
    {
        [JsonPropertyName("vehicle_types")]
        public ICollection<VehicleTypeDto> VehicleTypes { get; set; } = [];
    }
}