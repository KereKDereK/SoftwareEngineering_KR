using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class ConnectionPair
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("firstClient")]
        public string FirstClient { get; set; }
        [JsonProperty("secondClient")]
        public string SecondClient { get; set; }
        [JsonProperty("connectionWindows")]
        public List<Tuple<DateTime, DateTime>> ConnectionWindows { get; set; } 
        public ConnectionPair(string fclnt, string sclnt, List<Tuple<DateTime, DateTime>> windows)
        {
            FirstClient = fclnt;
            SecondClient = sclnt;
            ConnectionWindows = windows;
        }
    }

}
