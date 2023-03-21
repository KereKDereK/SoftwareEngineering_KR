using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class ScheduleGenerator
    {
        public List<Tuple<string, string>> ConnectionPairs { get; set; }

        public ScheduleGenerator()
        {
            try
            {
                ConnectionPairs = ConnectionPairsParseFromTxt();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[LOG] Schedule generator failed. Check connection pairs file");
            }
        }
        private List<Tuple<string, string>> ConnectionPairsParseFromTxt()
        {
            List<Tuple<string, string>> connectionPairs = new List<Tuple<string, string>>();
            string[] lines = System.IO.File.ReadAllLines(@"pairs.txt");
            foreach (string line in lines)
            {
                string[] subs = line.Split(' ');
                connectionPairs.Add(Tuple.Create(subs[0], subs[1]));
            }
            return connectionPairs;
        }
    }
}
