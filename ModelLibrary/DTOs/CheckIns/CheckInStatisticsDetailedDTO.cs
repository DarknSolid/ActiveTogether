using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.DTOs.CheckIns
{
    public class CheckInStatisticsDetailedDTO
    {
        /// e.g. Monday => "14" (hour) => 11 (people), 14 (dogs)
        public Dictionary<DayOfWeek, Dictionary<int, CheckInStatisticsHourTupleDTO>> IntraDayStatistics { get; set; }
        public int PeopleCheckIns { get; set; }
        public int DogCheckIns { get; set; }

        public CheckInStatisticsDetailedDTO()
        {
            IntraDayStatistics = new();
        }
    }
}
