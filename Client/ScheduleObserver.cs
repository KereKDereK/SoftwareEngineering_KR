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
        private string ServerIP { get; set; }

        public ScheduleObserver(string path)
        {
            PathToSchedule = path;
            Schedule = Schedule.ParseFromJson(PathToSchedule);
        }

        private async void Observe()
        {
            var serverport = 0;
            while (true)
            {
                foreach (ConnectionPair pair in Schedule.ConnectionPairs)
                    foreach (var window in pair.ConnectionWindows)
                        if (DateTime.Now >= window.Item1 && DateTime.Now <= window.Item2) 
                        { 
                            serverport = 25551 + (pair.Id * 2 - 1) + 0;
                            CreateConnection(serverport);
                        }
            }
        }
        private int CreateConnection(int port) 
        {
            var connection = new Connection(ServerIP, port);
            if (connection.flag)
            {
                Console.WriteLine("[ERROR] Connection faild");
                return 1;
            }
            return 0;
        }
    }
}
