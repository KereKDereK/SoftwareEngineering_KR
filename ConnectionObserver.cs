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
            CurrentConnections = new List<ConnectionEntity>();
            Generator = new ScheduleGenerator();
            CurrentSchedule = Generator.ScheduleParseFromJson(@"Schedule1.json");
            CheckSchedule();
        }
        private void CheckSchedule()
        {
            if (CurrentSchedule.EndTime < DateTime.Now)
            {
                Console.WriteLine("[LOG] Schedule was outdated. Generating new schedule...");
                Generator.GenerateSchedule();
                Generator.ScheduleParseToJson();
                CurrentSchedule = Generator.ScheduleParseFromJson(@"Schedule1.json");
            }
        }
        private void ManageConenctions()
        {

        }
    }
}
