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
        private Socket ServerConnection = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
        public bool flag = false;
        public Connection(string ip, int port)
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                ServerConnection.Connect(remoteEP);
            } 
            catch (SocketException se)
            {
                //Console.WriteLine("SocketException : {0}", se.ToString());
                remoteEP = new IPEndPoint(IPAddress.Parse(ip), port+1);
                try
                {
                    ServerConnection.Connect(remoteEP);
                }
                catch
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    flag = true;
                }
            }
        }

        private void Send()
        {
            Console.WriteLine("Input message to send:");
            var input = Console.ReadLine();
            input += " <EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(input);
            int bytesSent = ServerConnection.Send(msg);
        }
        private void Receive()
        {
            byte[] bytes = new byte[1024];
            int bytesRec = ServerConnection.Receive(bytes);
            Console.WriteLine("Echoed test = {0}",
                Encoding.ASCII.GetString(bytes, 0, bytesRec));
        }
        public void ObserveConnection()
        {
            while (ServerConnection.Connected)
            {
                Send();
                Receive();
            }
        }
    }
}
