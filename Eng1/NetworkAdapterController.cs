using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



namespace Eng1
{
    class NetworkAdapterController
    {
        public bool IsActive { get; set; }

        public NetworkAdapterController()
        {
            IsActive = true;
        }

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
        public void DisableAdapter()
        {
            if (IsActive)
            {
                string disableNet = "wmic path win32_networkadapter where PhysicalAdapter=True call disable";
                runCmdCommand(disableNet);
            }
            IsActive = false;
        }
        public void EnableAdapter()
        {
            if (!IsActive)
            {
                string enableNet = "wmic path win32_networkadapter where PhysicalAdapter=True call enable";
                runCmdCommand(enableNet);
            }
            IsActive = true;
        }
    }
}
