using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace ShadesTweaker
{
    public class StartupAppsManager
    {
        public List<StartupAppInfo> GetStartupApps()
        {
            List<StartupAppInfo> startupApps = new List<StartupAppInfo>();

            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMv2", "SELECT * FROM Win32_StartupCommand"))
                using (ManagementObjectCollection queryCollection = searcher.Get())
                {
                    foreach (ManagementObject mObject in queryCollection)
                    {
                        startupApps.Add(new StartupAppInfo
                        {
                            Location= mObject["Command"].ToString(),
                            Status = IsAppRunning(mObject["Name"].ToString()) ? "Running" : "Not Running",
                            User = mObject["User"].ToString(),
                        });
                    }
                }
            }
            catch (Exception )
            {

            }

            return startupApps;
        }

        private bool IsAppRunning(string appName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(appName);
                return processes.Length > 0;
            }
            catch
            {
                return false;
            }
        }

    }

    public class StartupAppInfo
    {
        public string Location { get; set; }
        public string Status { get; set; }
        public string User { get; set; }
    }
}
