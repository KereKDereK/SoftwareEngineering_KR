using System;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace Client
{

    // Client app is the one sending messages to a Server/listener.
    // Both listener and client can send messages back and forth once a
    // communication is established.
    public class SocketClient
    {
        public static int Main(String[] args)
        {
            var a = new ScheduleObserver(@"C:\Users\kerek\source\repos\Eng1\Eng1\bin\Debug\Schedule.json");
            a.Observe();
            return 0;
        }
    }
}
