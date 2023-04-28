using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
            catch { return "[ERROR] An error occured while changing connection status."; }
        }
        private static void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] An error occured while shutting down socket.");
            }
            finally
            {
                socket.Dispose();
            }
        }
        private void CheckConnection(Socket socket)
        {
            if (socket.RemoteEndPoint.ToString().Split(':')[0] != ConnectionPair.FirstClient)
            {
                Console.WriteLine("[CONNECTION] Invalid ip detected. Closing connection.");
                // CloseSocket(socket);
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
                var bytesRec = handler.Receive(buffer);
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
            var a = handler.Send(msg);
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
                handlerFirst = await FirstClientConnection.AcceptAsync();
                Console.WriteLine("[CONNECTION] First client remote IP is: {0}", handlerFirst.RemoteEndPoint);
                //CheckConnection(handlerFirst);
                Console.WriteLine("[CONNECTION] Waiting for the second client");
                handlerSecond =  await SecondClientConnection.AcceptAsync();
                Console.WriteLine("[CONNECTION] First client remote IP is: {0}", handlerSecond.RemoteEndPoint);
                //CheckConnection(handlerSecond);
            }
            string message;
            int data;
            while (ShouldBeActive)
            {
                message = await Task.FromResult(ReceiveData(handlerFirst));
                data = await Task.FromResult(SendData(handlerSecond, message));
                message = await Task.FromResult(ReceiveData(handlerSecond));
                data = await Task.FromResult(SendData(handlerFirst, message));
                Console.WriteLine("[CONNECTION] Connection (" + Id + ") is active");
            }
            CloseSocket(handlerFirst);
            CloseSocket(handlerSecond);
            CloseSocket(FirstClientConnection);
            CloseSocket(SecondClientConnection);
            IsActive = false;
        }
    }
}
