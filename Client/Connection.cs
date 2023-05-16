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
        private string key { get; set; }
        public bool flag = false;
        public Connection(string ip, int port, string currentKey)
        {
            RemoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            key = currentKey;
        }

        private async Task Send()
        {
            Console.WriteLine("Input message to send:");
            var input = Console.ReadLine();
            input = Crypto.Encrypt(input, key);
            input += " <EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(input);
            var bytesSent = ServerConnection.Send(msg, SocketFlags.None);
        }
        private async Task Receive()
        {
            byte[] bytes = new byte[1024];
            try
            {
                int bytesRec = ServerConnection.Receive(bytes, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
            string msg = Encoding.ASCII.GetString(bytes);
            msg = Crypto.Decrypt(msg, key);
            Console.WriteLine("Echoed test = {0}", msg);
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
                Console.WriteLine("[LOG] Connection stopped");
            }
            return Task.CompletedTask;
        }
    }
}
