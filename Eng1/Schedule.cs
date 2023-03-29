using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eng1
{
    public class Schedule
    {
        [JsonProperty("connections")]
        public List<ConnectionPair> ConnectionPairs { get; set; }
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }
        public Schedule(List<ConnectionPair> pairs, DateTime date)
        {
            ConnectionPairs = pairs;
            EndTime = date; 
        }
    }
}
