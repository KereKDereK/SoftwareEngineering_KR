using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ScheduleObserver
    {
        public string PathToSchedule { get; set; }
        Connection currentConnection { get; set; }
        NetworkAdapterController Adapter { get; set; } = new NetworkAdapterController();
        public Schedule Schedule { get; set; }  
        private Crypto Crypt { get; set; }
        private AESDecryptor FileDecryptor { get; set; } = new AESDecryptor();
        private string ServerIP { get; set; }
        private string HostIP { get; set; }
        private bool AssertionFlag { get; set; } = true;

        public ScheduleObserver(string path, string serverIp = "127.0.0.1")
        {
            PathToSchedule = path;
            string enc = "Schedule.enc";
            string dec = "Schedule.json";
            try
            {
                if (!File.Exists(PathToSchedule) && File.Exists(enc))
                {
                        FileDecryptor.DecryptShedule(enc, dec);
                }
                else if (!File.Exists(PathToSchedule) && !File.Exists(enc))
                    AssertionFlag = false;
                Schedule = Schedule.ParseFromJson(PathToSchedule);
            }
            catch(FileNotFoundException ex )
            {
                Console.WriteLine("[ERROR] Schedule file not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] An error occured while parsing schedule. Please, check your schedule file correctness");
            }
            Crypt = new Crypto("", Schedule, HostIP);
            HostIP = FindHostIp();
            ServerIP = ServerParseFromTxt();
            Console.WriteLine(HostIP);
        }
        private string ServerParseFromTxt()
        {
            string result = "127.0.0.1";
            try
            {
                result = File.ReadAllText(@"server.txt");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("[ERROR] Server file not found.");
            }
            Console.WriteLine("[LOG] Txt parsed successfully");
            return result;
        }
        private string FindHostIp()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }
        private bool CheckSchedule()
        {
            if (Schedule.EndTime < DateTime.Now)
            {
                Console.WriteLine("[LOG] Schedule was outdated. Contact your system administrator for the new one. Terminating");
                return false;
            }
            else
                return true;
        }
        public async void Observe()
        {
            int serverport = 0;
            bool flag = false;
            while (true)
            {
                if (AssertionFlag == false || Schedule == null)
                    break;
                if (!CheckSchedule())
                {
                    Adapter.DisableAdapter();
                    break;
                }
                foreach (ConnectionPair pair in Schedule.ConnectionPairs)
                    foreach (var window in pair.ConnectionWindows)
                        if (DateTime.Now >= window.Item1 && DateTime.Now <= window.Item2) 
                        {
                            flag = true;
                            Adapter.EnableAdapter();
                            string currentKey = "";
                            if (HostIP == pair.FirstClient)
                            {
                                serverport = 25551 + (pair.Id * 2 - 1) + 0;
                                currentKey = Crypt.ipToKey[pair.SecondClient];
                            }
                            else
                            {
                                serverport = 25551 + (pair.Id * 2 - 1) + 1; // заменить на + 1
                                currentKey = Crypt.ipToKey[pair.FirstClient];
                            }
                            try
                            {
                                
                                CreateConnection(serverport, currentKey, window.Item2);
                            }
                            catch (SocketException ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("[ERROR] An error occured while establishing connection");
                                Console.WriteLine(ex.ToString());
                            }
                        }
                if (flag == false)
                    Adapter.DisableAdapter();
                flag = false;

            }
            Console.ReadKey();
        }
        private async Task<int> CreateConnection(int port, string currentKey, DateTime window) 
        {
            if (currentConnection == null)
            {
                currentConnection = new Connection(ServerIP, port, currentKey);
                currentConnection.SetConnectionEnd(window);
            }
            if (currentConnection.flag)
            {
                Console.WriteLine("[ERROR] Connection failed");
                return 1;
            }
            try
            {
                if (currentConnection.IsActive == false)
                    currentConnection.ObserveConnection();
            }
            catch(SocketException ex)
            {
                Console.WriteLine("[LOG] Server cancelled connection.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("[LOG] An error occured during connection.");
            }
            return 0;
        }
    }
}
