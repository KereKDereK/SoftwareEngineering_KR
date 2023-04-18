using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class ScheduleGenerator
    {
        public List<Tuple<string, string>> ConnectionPairs { get; set; }
        private DateTime StartDate { get; set; }
        private DateTime EndDate { get; set; }
        private Random Gen { get; set; }
        private Schedule Schedule { get; set; }

        public ScheduleGenerator()
        {
            StartDate = DateTime.Now.Date;   
            EndDate = StartDate.AddDays(30);
            Gen = new Random();
            try
            {
                ConnectionPairs = ConnectionPairsParseFromTxt();
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] Schedule generator failed. Check connection pairs file");
            }
        }
        private List<Tuple<string, string>> ConnectionPairsParseFromTxt()
        {
            List<Tuple<string, string>> connectionPairs = new List<Tuple<string, string>>();
            string[] lines = File.ReadAllLines(@"pairs.txt");
            foreach (string line in lines)
            {
                string[] subs = line.Split(' ');
                connectionPairs.Add(Tuple.Create(subs[0], subs[1]));
            }
            Console.WriteLine("[LOG] Schedule generator parsed txt successfully");
            return connectionPairs;
        }
        private DateTime GenerateRandomDateTime(DateTime Day)
        {
            return Day.AddHours(Gen.Next(12, 18)).AddMinutes(Gen.Next(0, 60)).AddSeconds(Gen.Next(0, 60));
        }
        public void GenerateSchedule()
        {
            var connectionPairs = new List<ConnectionPair>();
            DateTime endTime = new DateTime();
            foreach (Tuple<string, string> pair in ConnectionPairs)
            {
                var pairschedule = new List<Tuple<DateTime, DateTime>>();
                for (int i = 0; i < 30; i++)
                {
                    var leftdate = GenerateRandomDateTime(StartDate.AddDays(i));
                    var rightdate = leftdate.AddMinutes(5);
                    pairschedule.Add(new Tuple<DateTime, DateTime>(leftdate, rightdate));
                    endTime = rightdate.Date.AddDays(1);
                }
                connectionPairs.Add(new ConnectionPair(pair.Item1, pair.Item2, pairschedule));
            }
            Schedule = new Schedule(connectionPairs, endTime);
            Console.WriteLine("[LOG] Schedule successfully generated");
        }
        public void ScheduleParseToJson()
        {
            string fileName = "Schedule.json";
            string jsonString = JsonConvert.SerializeObject(Schedule);
            File.WriteAllText(fileName, jsonString);
            Console.WriteLine("[LOG] Schedule successfully parsed to json file");
        }

        public Schedule ScheduleParseFromJson(string filepath)
        {
            Schedule = Schedule.ParseFromJson(filepath);
            return Schedule;
        }
    }
}
