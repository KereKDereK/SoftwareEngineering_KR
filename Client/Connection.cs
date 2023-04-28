using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Connection
    {
        private Socket ServerConnection { get; set; }
        private IPEndPoint RemoteEP { get; set; }
        public bool flag = false;
        public Connection(string ip, int port)
        {
            RemoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        private async Task Send()
        {
            Console.WriteLine("Input message to send:");
            var input = Console.ReadLine();
            input += " <EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(input);
            int bytesSent = ServerConnection.Send(msg, SocketFlags.None);
        }
        private async Task Receive()
        {
            byte[] bytes = new byte[1024];
            int bytesRec = ServerConnection.Receive(bytes, SocketFlags.None);
            Console.WriteLine("Echoed test = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));
        }
        public async Task<Task> ObserveConnection()
        {
            using (ServerConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                ServerConnection.Connect(RemoteEP);
                while (ServerConnection.Connected)
                {
                    await Send();
                    await Receive();
                }
            }
            return Task.CompletedTask;
        }
    }
}
