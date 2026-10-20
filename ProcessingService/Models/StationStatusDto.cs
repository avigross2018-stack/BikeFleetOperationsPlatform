using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProcessingService.Models
{
    public class StationStatusDto
    {
        public string StationId { get; set; } = string.Empty;
        public int NumBikesAvailable { get; set; }
        public int NumBikesDisabled { get; set; }
        public int NumDocksAvailable { get; set; }
        public int NumDocksDisabled { get; set; }
        public int NumEbikesAvailable { get; set; }
        public int IsInstalled { get; set; }
        public int IsRenting { get; set; }
        public int IsReturning { get; set; }
        public long LastReported { get; set; }
    }
}