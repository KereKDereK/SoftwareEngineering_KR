using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace Client
{
    class NetworkAdapterController
    {
        private static void runCmdCommand(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = $"/C {cmd}";
            process.StartInfo = startInfo;
            process.Start();
        }
        public static void DisableAdapter()
        {
            string disableNet = "wmic path win32_networkadapter where PhysicalAdapter=True call disable";
            runCmdCommand(disableNet);
        }
        public static void EnableAdapter()
        {
            string enableNet = "wmic path win32_networkadapter where PhysicalAdapter=True call enable";
            runCmdCommand(enableNet);
        }

    }
}
