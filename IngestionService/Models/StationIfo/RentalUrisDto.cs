using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.StationIfo
{
    public class RentalUrisDto
    {
        [JsonPropertyName("android")]
        public string? Android { get; set; }

        [JsonPropertyName("ios")]
        public string? Ios { get; set; }        
    }
}