
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eng1
{
    public class ConnectionObserver
    {
        public int ConnectionId { get; set; }
        private Schedule CurrentSchedule { get; set; }
        private ScheduleGenerator Generator { get; set; }
        private NetworkAdapterController Controller { get; set; } = new NetworkAdapterController();
        private List<ConnectionEntity> CurrentConnections { get; set; }

        public ConnectionObserver()
        {
            ConnectionId = 0;
            CurrentConnections = new List<ConnectionEntity>();
            Generator = new ScheduleGenerator();
            CurrentSchedule = Generator.ScheduleParseFromJson(@"Schedule.json");
            CheckSchedule();
            foreach (ConnectionPair pair in CurrentSchedule.ConnectionPairs)
            {
                CurrentConnections.Add(new ConnectionEntity(ConnectionId, false, pair));
                ++ConnectionId;
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
        public async void ManageConnectionsAsync()
        {
            var globalFlag = false;
            var connectionFlag = false;
            while (true)
            {
                CheckSchedule();
                globalFlag = false;
                foreach (ConnectionEntity connection in CurrentConnections)
                {
                    connectionFlag = false;
                    foreach(Tuple<DateTime, DateTime> window in connection.ConnectionPair.ConnectionWindows)
                    {
                        if (DateTime.Now >= window.Item1 && DateTime.Now <= window.Item2)
                        {
                            connection.SetConnectionEnd(window.Item2);
                            Controller.EnableAdapter();
                            connectionFlag = true;
                            globalFlag = true;
                            break;
                        }
                    }
                    if (connectionFlag)
                    {
                        connection.ConnectionStatusChange(true);
                        if (!connection.IsActive)
                            connection.ActivateConnection();
                    }
                }
                if (!globalFlag)
                {
                    Console.WriteLine("[ADAPTER] Internet adapter is down");
                    Controller.DisableAdapter();
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
