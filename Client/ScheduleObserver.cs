﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ScheduleObserver
    {
        public string PathToSchedule { get; set; }
        Connection currentConnection { get; set; }
        public Schedule Schedule { get; set; }  
        private Crypto Crypt { get; set; }
        private string ServerIP { get; set; }
        private string HostIP { get; set; }

        public ScheduleObserver(string path)
        {
            PathToSchedule = path;
            Schedule = Schedule.ParseFromJson(PathToSchedule);
            Crypt = new Crypto("", Schedule, HostIP);
            string host = Dns.GetHostName();
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            HostIP = localIP;
            ServerIP = "127.0.0.1";
            Console.WriteLine(HostIP);
        }

        public async void Observe()
        {
            int serverport = 0;
            while (true)
            {
                foreach (ConnectionPair pair in Schedule.ConnectionPairs)
                    foreach (var window in pair.ConnectionWindows)
                        if (DateTime.Now >= window.Item1 && DateTime.Now <= window.Item2) 
                        {
                            string currentKey = "";
                            if (HostIP == pair.FirstClient)
                            {
                                serverport = 25551 + (pair.Id * 2 - 1) + 1;
                                currentKey = Crypt.ipToKey[pair.SecondClient];
                            }
                            else
                            {
                                serverport = 25551 + (pair.Id * 2 - 1) + 1; // заменить на + 1
                                currentKey = Crypt.ipToKey[pair.FirstClient];
                            }
                            Console.WriteLine(serverport);
                            try
                            {
                                await CreateConnection(serverport, currentKey);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[ERROR] An error occured while establishing connection");
                            }
                        }
            }
        }
        private async Task<int> CreateConnection(int port, string currentKey) 
        {
            if (currentConnection == null)
            {
                currentConnection = new Connection(ServerIP, port, currentKey);
                return 0;
            }
            if (currentConnection.flag)
            {
                Console.WriteLine("[ERROR] Connection failed");
                return 1;
            }
            await currentConnection.ObserveConnection();
            return 0;
        }
    }
}
