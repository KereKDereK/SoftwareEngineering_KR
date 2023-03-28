using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace Eng1
{
    public class ConnectionEntity
    {
        private int Id { get; set; }
        private ConnectionPair ConnectionPair { get; set; }
        private Socket FirstClientConnection { get; set; }
        private Socket SecondClientConnection { get; set; }
        private bool ShouldBeActive { get; set; }
        public ConnectionEntity (int id, bool flag, ConnectionPair connectionPair)
        {
            Id = id;
            ShouldBeActive = flag;
            ConnectionPair = connectionPair;
            ActiveConnection();
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
        private void CloseSocket()
        {
            try
            {
                FirstClientConnection.Shutdown(SocketShutdown.Both);
                SecondClientConnection.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                FirstClientConnection.Close();
                SecondClientConnection.Close();
            }
        }
        private void ConnectionInit()
        {
            if (ShouldBeActive)
            {
                FirstClientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SecondClientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                CloseSocket();
            }
        }
        private void ActiveConnection()
        {
            ConnectionInit();
            while (ShouldBeActive)
            {
                Console.WriteLine("[LOG] Connection (" + Id + ") is active");
                Thread.Sleep(3000);
            }
        }
    }
}
