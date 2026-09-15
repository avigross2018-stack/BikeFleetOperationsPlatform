using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IngestionService.Models.StationStatus
{
    public class StationStatusData
    {
        public ICollection<StationStatusDto> Stations { get; set; } = [];
    }
}