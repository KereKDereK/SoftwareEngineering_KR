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
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("connections")]
        public List<ConnectionPair> ConnectionPairs { get; set; }
        [JsonProperty("endTime")]
        private DateTime EndTime { get; set; }
        public Schedule(int id, List<ConnectionPair> pairs, DateTime date)
        {
            Id = id;
            ConnectionPairs = pairs;
            EndTime = date;
        }
    }
}
