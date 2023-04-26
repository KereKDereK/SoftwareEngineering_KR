using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Eng1
{
    public class ConnectionEntity
    {
        private int Id { get; set; }
        public ConnectionPair ConnectionPair { get; set; }
        public bool IsActive { get; set; }
        private Socket FirstClientConnection { get; set; }
        private Socket SecondClientConnection { get; set; }
        private bool ShouldBeActive { get; set; }
        public ConnectionEntity (int id, bool flag, ConnectionPair connectionPair)
        {
            Id = id;
            ShouldBeActive = flag;
            ConnectionPair = connectionPair;
        }
        public string ConnectionStatusChange(bool state)
        {
            try
            {
                ShouldBeActive = state;
                return "[LOG] Status of connection (" + Id + ") changed successfully to " + ShouldBeActive;
            }
            catch { return "[ERROR] An error occured while changing connection status."; Thread.Sleep(1000); }
        }
        private void CloseSockets()
        {
            try
            {
                FirstClientConnection.Shutdown(SocketShutdown.Both);
                SecondClientConnection.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] An error occured while shutting down sockets.");
            }
            finally
            {
                FirstClientConnection.Dispose();
                SecondClientConnection.Dispose();
                IsActive = false;
            }
        }
        private void ConnectionInit()
        {
            if (ShouldBeActive)
            {
                try
                {

                    FirstClientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    FirstClientConnection.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25551 + (Id*2 - 1)));
                    FirstClientConnection.Listen(1);

                    SecondClientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    SecondClientConnection.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25551 + (Id * 2)));
                    SecondClientConnection.Listen(1);

                    IsActive = true;
                }
                catch(SocketException)
                {
                    Console.WriteLine("[ERROR] An error occured while initializing sockets.");
                }
            }
        }
        private string ReceiveData(Socket handler)
        {
            byte[] buffer = null;
            string data = null;
            while (true)
            {
                buffer = new byte[1024];
                int bytesRec = handler.Receive(buffer);
                data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {
                    return data;
                }
            }
        }
        private int SendData(Socket handler, string data)
        {
            byte[] msg = Encoding.ASCII.GetBytes(data);
            handler.Send(msg);
            return 1;
        }
        public async Task ActivateConnection()
        {
            ConnectionInit();
            Socket handlerFirst = null;
            Socket handlerSecond = null;
            if (ShouldBeActive)
            {
                Console.WriteLine("[CONNECTION] Waiting for the first client");
                handlerFirst = FirstClientConnection.Accept();
                //Console.WriteLine("[CONNECTION] Waiting for the second client");
                //handlerSecond = SecondClientConnection.Accept();
            }
            while (ShouldBeActive)
            {
                string result = await Task.FromResult(ReceiveData(handlerFirst));
                var asasdas = await Task.FromResult(SendData(handlerFirst, result));
                //await Task.FromResult(ReceiveData(handlerSecond));
                Console.WriteLine("[CONNECTION] Connection (" + Id + ") is active");
            }
            CloseSockets();
        }
    }
}
