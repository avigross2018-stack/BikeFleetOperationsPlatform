using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProcessingService.Models
{
    public class StationInfoDto
    {
        public string StationId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? RegionId { get; set; }
        public int Capacity { get; set; }

    }
}