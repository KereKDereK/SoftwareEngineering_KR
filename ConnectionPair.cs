using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eng1
{
    public class ConnectionPair
    {
        public int Id { get; set; }
        public string FirstClient { get; set; }
        public string SecondClient { get; set; }
        public List<DateTime> ConnectionWindows { get; set; } 
        public ConnectionPair(int id, string fclnt, string sclnt, List<DateTime> windows)
        {
            Id = id;
            FirstClient = fclnt;
            SecondClient = sclnt;
            ConnectionWindows = windows;
        }
    }

}
