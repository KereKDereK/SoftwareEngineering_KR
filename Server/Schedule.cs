using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
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
        public static Schedule ParseFromJson(string filepath)
        {
            Schedule scheduleToUpdate;
            try
            {
                scheduleToUpdate = JsonConvert.DeserializeObject<Schedule>(File.ReadAllText(filepath));
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] An error occured. Schedule file does not exist");
                return new Schedule(new List<ConnectionPair>(), DateTime.MinValue);
            }
            Console.WriteLine("[LOG] Schedule successfully parsed from json file");
            return scheduleToUpdate;
        }    
    }
}
