using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class Schedule
    {
        public int Id { get; set; }
        public List<ConnectionPair> ConnectionPairs { get; set; }
        private DateTime EndTime { get; set; }

        Schedule(int id, string filepath)
        {
            Id = id;
            ScheduleParseFromJson(filepath);
        }
        
        private void ScheduleParseFromJson(string filepath)
        {
            //парсим все
        }
    }
}
