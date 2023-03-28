using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class ConnectionObserver
    {
        private Schedule CurrentSchedule { get; set; }
        private ScheduleGenerator Generator { get; set; }
        private List<ConnectionEntity> CurrentConnections { get; set; }

        public ConnectionObserver()
        {
            var id = 0;
            CurrentConnections = new List<ConnectionEntity>();
            Generator = new ScheduleGenerator();
            CurrentSchedule = Generator.ScheduleParseFromJson(@"Schedule.json");
            CheckSchedule();
            foreach (ConnectionPair pair in CurrentSchedule.ConnectionPairs)
            {
                CurrentConnections.Add(new ConnectionEntity(id, true, pair));
            }
        }
        private void CheckSchedule()
        {
            if (CurrentSchedule.EndTime < DateTime.Now)
            {
                Console.WriteLine("[LOG] Schedule was outdated. Generating new schedule...");
                Generator.GenerateSchedule();
                Generator.ScheduleParseToJson();
                CurrentSchedule = Generator.ScheduleParseFromJson(@"Schedule.json");
            }
        }
        public async void ManageConnections()
        {

        }
    }
}
