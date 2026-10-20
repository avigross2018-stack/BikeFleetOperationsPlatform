using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IngestionService.Models.StationIfo
{
    public class StationInfoData
    {
         [JsonPropertyName("stations")]
        public List<StationInfoDto> Stations { get; set; } = [];   
    }
}