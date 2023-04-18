using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ScheduleObserver
    {
        public string PathToSchedule { get; set; }
        public Schedule Schedule { get; set; }  

        public ScheduleObserver(string path)
        {
            PathToSchedule = path;
            Schedule = Schedule.ParseFromJson(PathToSchedule);
        }

        private int Observe()
        {
            while (true)
            {
                foreach (ConnectionPair pair in Schedule.ConnectionPairs)
                    foreach (var window in pair.ConnectionWindows)
                        if (DateTime.Now >= window.Item1 && DateTime.Now <= window.Item2)
                            break; // дописать
            }
            return 0;
        }
    }
}
