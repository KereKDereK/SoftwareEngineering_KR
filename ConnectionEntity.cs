using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Eng1
{
    public class ConnectionEntity
    {
        private short Id { get; set; }
        private string FirstClientIp { get; set; }
        private Socket FirstClientConnection { get; set; }
        private string SecondClientIp { get; set; }
        private Socket SecondClientConnection { get; set; }
        private bool ShouldBeActive { get; set; }
        public ConnectionEntity (short id, bool flag, string firstip, string secondip)
        {
            Id = id;
            ShouldBeActive = flag;
            FirstClientIp = firstip;
            SecondClientIp = secondip;
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
            while (true)
            {
                ConnectionInit();
                while (ShouldBeActive)
                {
                    Console.WriteLine("[LOG] Connection (" + Id + ") is active");
                }
            }
        }
    }
}
