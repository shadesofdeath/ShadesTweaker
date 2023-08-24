using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wpf.Ui.Controls;


namespace ShadesTweaker
{
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                if (IsWindows11OrHigher())
                {
                    SetAcrylicTheme();
                }
                else
                {
                    SetTabbedTheme();
                }
            };
        }

        private bool IsWindows11OrHigher()
        {
            // Windows 11 version number: 10.0.22000
            Version win11Version = new Version(10, 0, 22000, 0);
            return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                   Environment.OSVersion.Version >= win11Version;
        }

        private void SetAcrylicTheme()
        {
            Wpf.Ui.Appearance.Watcher.Watch(this, Wpf.Ui.Appearance.BackgroundType.Acrylic, true);
        }

        private void SetTabbedTheme()
        {
            Wpf.Ui.Appearance.Watcher.Watch(this, Wpf.Ui.Appearance.BackgroundType.Tabbed, true);
        }

        public class ToggleSwitchState
        {
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }
        private List<ToggleSwitchState> toggleSwitchStates = new List<ToggleSwitchState>();
        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                var existingState = toggleSwitchStates.FirstOrDefault(state => state.Name == toggleSwitch.Name);
                if (existingState != null)
                {
                    existingState.IsChecked = true;
                }
                else
                {
                    toggleSwitchStates.Add(new ToggleSwitchState { Name = toggleSwitch.Name, IsChecked = true });
                }
            }
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                var existingState = toggleSwitchStates.FirstOrDefault(state => state.Name == toggleSwitch.Name);
                if (existingState != null)
                {
                    existingState.IsChecked = false;
                }
                else
                {
                    toggleSwitchStates.Add(new ToggleSwitchState { Name = toggleSwitch.Name, IsChecked = false });
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    XDocument xmlDoc = XDocument.Load(openFileDialog.FileName);
                    IEnumerable<XElement> toggleSwitchElements = xmlDoc.Descendants("ToggleSwitchState");

                    foreach (XElement toggleSwitchElement in toggleSwitchElements)
                    {
                        string toggleSwitchName = toggleSwitchElement.Attribute("Name").Value;
                        bool isChecked = bool.Parse(toggleSwitchElement.Value);

                        ToggleSwitch toggleSwitch = FindToggleSwitchByName(toggleSwitchName);
                        if (toggleSwitch != null)
                        {
                            toggleSwitch.IsChecked = isChecked;
                        }
                    }

                    System.Windows.MessageBox.Show("Preset file imported successfully :)");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error occurred while importing: " + ex.Message);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    XDocument xmlDoc = new XDocument(new XElement("ToggleSwitchStates"));

                    foreach (TabItem tabItem in myTabControl.Items)
                    {
                        if (tabItem.Content is Panel panel)
                        {
                            foreach (StackPanel stackPanel in panel.Children.OfType<StackPanel>())
                            {
                                foreach (ToggleSwitch toggleSwitch in stackPanel.Children.OfType<ToggleSwitch>())
                                {
                                    xmlDoc.Root.Add(new XElement("ToggleSwitchState",
                                        new XAttribute("Name", toggleSwitch.Name),
                                        toggleSwitch.IsChecked.ToString()));
                                }
                            }
                        }
                    }

                    xmlDoc.Save(saveFileDialog.FileName);
                    System.Windows.MessageBox.Show("Preset file saved successfully :)");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error occurred while exporting: " + ex.Message);
            }
        }




        private ToggleSwitch FindToggleSwitchByName(string name)
        {
            foreach (TabItem tabItem in myTabControl.Items)
            {
                if (tabItem.Content is Panel panel)
                {
                    foreach (UIElement uiElement in panel.Children)
                    {
                        if (uiElement is StackPanel stackPanel)
                        {
                            foreach (UIElement toggleSwitchElement in stackPanel.Children)
                            {
                                if (toggleSwitchElement is ToggleSwitch toggleSwitch && toggleSwitch.Name == name)
                                {
                                    return toggleSwitch;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        // Error Reporting Service
        private void ErrorReportingToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);
        }
        private void ErrorReportingToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 0, RegistryValueKind.DWord);
        }

        // Modern Syandby
        private void modernStandbyToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "PlatformAoAcOverride", 0, RegistryValueKind.DWord);
        }
        private void modernStandbyToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "PlatformAoAcOverride");
        }

        // Fast Startup
        private void fastStartupToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 1, RegistryValueKind.DWord);

        }
        private void fastStartupToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 0, RegistryValueKind.DWord);

        }

        // Memory Compression
        private void disableMemoryCompressionToggle_Checked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Disable-MMAgent -mc\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        private void disableMemoryCompressionToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Enable-MMAgent -mc\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        // Storage Sense
        private void storagesenseToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\StorageSense", "AllowStorageSenseGlobal", 1, RegistryValueKind.DWord);

        }
        private void storagesenseToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\StorageSense", "AllowStorageSenseGlobal");

        }

        //Program Compatibility Assistant
        private void compatibilityassistansToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisablePCA", 1, RegistryValueKind.DWord);

        }
        private void compatibilityAssistantToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "DisablePCA");

        }

        // Print Spooler
        private void printspoolerToggle_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Spooler", "Start", 4, RegistryValueKind.DWord);
        }

        private void printspoolerToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Spooler", "Start", 2, RegistryValueKind.DWord);
        }

        // SysMain
        private void SysMain_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 4, RegistryValueKind.DWord);
        }

        private void SysMain_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 2, RegistryValueKind.DWord);
        }

        // Defender
        private void defender_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
        }

        private void defender_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 0, RegistryValueKind.DWord);
        }

        // Defender Real Time
        private void defender_real_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableRealtimeMonitoring", 1, RegistryValueKind.DWord);
        }

        private void defender_real_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableRealtimeMonitoring", 0, RegistryValueKind.DWord);
        }

        // Defender Antivirus
        private void defender_antivirus_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiVirus", 1, RegistryValueKind.DWord);
        }

        private void defender_antivirus_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiVirus", 0, RegistryValueKind.DWord);
        }

        // Smartscreen
        private void smartscreen_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", "Off", RegistryValueKind.String);
        }

        private void smartscreen_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", "SmartScreenEnabled", "On", RegistryValueKind.String);
        }

        // SystemRestore
        private void restore_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\SystemRestore", "DisableSR", 1, RegistryValueKind.DWord);
        }

        private void restore_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows NT\SystemRestore", "DisableSR", 0, RegistryValueKind.DWord);
        }

        // Superfetch
        private void superfetch_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnableSuperfetch", 0, RegistryValueKind.DWord);
        }

        private void superfetch_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnableSuperfetch", 1, RegistryValueKind.DWord);
        }

        // Hibernation
        private void hibernation_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 1, RegistryValueKind.DWord);
        }

        private void hibernation_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 0, RegistryValueKind.DWord);
        }

        // Disable NTFS Last Access Time Stamp
        private void ntfstime_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsDisableLastAccessUpdate", 80000003, RegistryValueKind.DWord);
        }

        private void ntfstime_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsDisableLastAccessUpdate", 80000002, RegistryValueKind.DWord);
        }

        // Disable Delivery Optimization
        private void delivery_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DoSvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void delivery_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DoSvc", "Start", 2, RegistryValueKind.DWord);
        }

        // Disable Auto-restart Notifications for Windows Update
        private void AutoRestart_Notifications_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "RestartNotificationsAllowed2", 0, RegistryValueKind.DWord);
        }

        private void AutoRestart_Notifications_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WindowsUpdate\UX\Settings", "RestartNotificationsAllowed2", 1, RegistryValueKind.DWord);
        }

        // Disable Include Drivers for Windows Update
        private void IncludeDrivers_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate", 1, RegistryValueKind.DWord);
        }

        private void IncludeDrivers_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "ExcludeWUDriversInQualityUpdate");
        }

        // Disable Feature Update
        private void FeatureUpdate_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DisableWUfBSafeguard", 1, RegistryValueKind.DWord);
        }

        private void FeatureUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate", "DisableWUfBSafeguard");
        }

        // Block Microsoft Edge Automatic Install
        private void blockedge_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\EdgeUpdate", "DoNotUpdateToEdgeWithChromium", 1, RegistryValueKind.DWord);
        }

        private void blockedge_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\EdgeUpdate", "DoNotUpdateToEdgeWithChromium");
        }

        // Disable Defender SmartScreen for Microsoft Edge
        private void ASS_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent", 2, RegistryValueKind.DWord);
        }

        private void ASS_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet", "SubmitSamplesConsent");
        }

        // Disable Defender Firewall
        private void firewall_Checked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled False\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        private void firewall_Unchecked(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled True\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        // ContextMenu Bat File
        private void bat_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.ClassesRoot, @".bat\ShellNew", "NullFile", "", RegistryValueKind.String);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled True\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        private void bat_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.ClassesRoot, @".bat\ShellNew", "NullFile");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled True\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        // ContextMenu CopyAsPath
        private void copypath_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Classes\AllFilesystemObjects\shellex\ContextMenuHandlers\CopyAsPathMenu", "", "{f3d06e7c-1e45-4a26-847e-f9fcdee59be0}", RegistryValueKind.String);
        }

        private void copypath_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Classes\AllFilesystemObjects\shellex\ContextMenuHandlers\CopyAsPathMenu", "", "", RegistryValueKind.String);
        }

        // ContextMenu Reg File
        private void vbs_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.ClassesRoot, @".vbs\PersistentHandler", "", "{5e941d80-bf96-11cd-b579-08002b30bfeb}", RegistryValueKind.String);

            RegHelper.SetValue(RegistryHive.ClassesRoot, @".vbs\ShellNew", "", "", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @".vbs\ShellNew", "ItemName", "@%SystemRoot%\\System32\\wshext.dll,-4802", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @".vbs\ShellNew", "NullFile", "", RegistryValueKind.String);
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled True\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        private void vbs_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.ClassesRoot, @".vbs\PersistentHandler", "", "", RegistryValueKind.String);
            RegHelper.TryDeleteValue(RegistryHive.ClassesRoot, @".vbs\ShellNew", "ItemName");
            RegHelper.TryDeleteValue(RegistryHive.ClassesRoot, @".vbs\ShellNew", "NullFile");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = "-ExecutionPolicy Bypass -Command \"Set-NetFirewallProfile -Enabled True\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = psi };
            process.Start();
        }

        // ContextMenu Remove "NVIDIA Control Panel"
        private void nvidia_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\NVIDIA Corporation\Global\NvCplApi\Policies", "ContextUIPolicy", 0, RegistryValueKind.DWord);
        }

        private void nvidia_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\NVIDIA Corporation\Global\NvCplApi\Policies", "ContextUIPolicy", 2, RegistryValueKind.DWord);
        }

        // ContextMenu TakeOwnership
        private void ownership_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership", "", "Take Ownership", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership", "AppliesTo", "NOT (System.ItemPathDisplay:=\"C:\\Users\" OR System.ItemPathDisplay:=\"C:\\ProgramData\" OR System.ItemPathDisplay:=\"C:\\Windows\" OR System.ItemPathDisplay:=\"C:\\Windows\\System32\" OR System.ItemPathDisplay:=\"C:\\Program Files\" OR System.ItemPathDisplay:=\"C:\\Program Files (x86)\")", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership", "HasLUAShield", "", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership", "NoWorkingDirectory", "", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership", "Position", "middle", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership\command", "", "powershell -windowstyle hidden -command \\\"Start-Process cmd -ArgumentList '/c takeown /f \\\\\\\"%1\\\\\\\" /r /d y && icacls \\\\\\\"%1\\\\\\\" /grant *S-1-3-4:F /t /c /l /q' -Verb runAs\\\"\"", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"directory\\shell\TakeOwnership\command", "IsolatedCommand", "powershell -windowstyle hidden -command \\\"Start-Process cmd -ArgumentList '/c takeown /f \\\\\\\"%1\\\\\\\" /r /d y && icacls \\\\\\\"%1\\\\\\\" /grant *S-1-3-4:F /t /c /l /q' -Verb runAs\\\"\"", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\shell\runas", "", "Take Ownership", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\shell\runas", "AppliesTo", "NOT (System.ItemPathDisplay:=\"C:\\\")", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\\shell\runas", "HasLUAShield", "", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\\shell\runas", "NoWorkingDirectory", "", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\\shell\runas", "Position", "middle", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\\shell\runas\command", "", "cmd.exe /c takeown /f \"%1\\\" /r /d y && icacls \"%1\\\" /grant *S-1-3-4:F /t /c", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Drive\\shell\runas\command", "IsolatedCommand", "cmd.exe /c takeown /f \"%1\\\" /r /d y && icacls \"%1\\\" /grant *S-1-3-4:F /t /c", RegistryValueKind.String);
        }


        private void ownership_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteSubKeyTree(RegistryHive.ClassesRoot, @"directory\shell\TakeOwnership");
            RegHelper.TryDeleteSubKeyTree(RegistryHive.ClassesRoot, @"drive\shell\runas");
        }

        // Old Context Menu
        private void oldcontextmenu_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32", "", "", RegistryValueKind.String);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void oldcontextmenu_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, @"CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}");
        }

        // Widgets
        private void widgets_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Dsh", "AllowNewsAndInterests", 0, RegistryValueKind.DWord);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void widgets_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Dsh", "AllowNewsAndInterests");
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        // Disable Peek
        private void peek_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\DWM", "EnableAeroPeek", 0, RegistryValueKind.DWord);
        }

        private void peek_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\DWM", "EnableAeroPeek", 1, RegistryValueKind.DWord);
        }

        // Remove Microsoft Teams Icon on Taskbar
        private void teams_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", 0, RegistryValueKind.DWord);
        }

        private void teams_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", 1, RegistryValueKind.DWord);
        }

        // Remove Microsoft Teams Icon on Taskbar
        private void bing_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 0, RegistryValueKind.DWord);
        }

        private void bing_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 1, RegistryValueKind.DWord);
        }

        // Dark Mode
        private void darkMode_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 0, RegistryValueKind.DWord);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void darkMode_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1, RegistryValueKind.DWord);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        // Transparency Effects
        private void transparency_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 0, RegistryValueKind.DWord);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void transparency_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", 1, RegistryValueKind.DWord);
            Process.Start("explorer.exe");
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        // // Show File Extensions
        private void showExtensions_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, RegistryValueKind.DWord);
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void showExtensions_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 1, RegistryValueKind.DWord);
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        // Increase Taskbar Thumbnail Size
        private void increaseThumbnailSize_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband", "MaxThumbSizePx", 500, RegistryValueKind.DWord);
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        private void increaseThumbnailSize_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband", "MaxThumbSizePx", 250, RegistryValueKind.DWord);
            string script = @"
        Stop-Process -Name explorer -Force
        Start-Process explorer
    ";
            using (Process ps = new Process())
            {
                ps.StartInfo.FileName = "powershell.exe";
                ps.StartInfo.Arguments = $"-Command \"{script}\"";
                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ps.Start();
                ps.WaitForExit();
            }
        }

        // Disable Lock Screen
        private void disableLockScreen_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 1, RegistryValueKind.DWord);
        }

        private void disableLockScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"Software\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 0, RegistryValueKind.DWord);
        }

        // Adjust Menu Show Delay
        private void adjustMenuDelay_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Control Panel\Desktop", "MenuShowDelay", 50, RegistryValueKind.String);
        }

        private void adjustMenuDelay_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Control Panel\Desktop", "MenuShowDelay", 400, RegistryValueKind.String);
        }

        // Disable Cortana
        private void disableCortana_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
        }

        private void disableCortana_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 1, RegistryValueKind.DWord);
        }

        // Disable Automatic App Updates
        private void disableAppUpdates_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Store", "AutoDownload", 2, RegistryValueKind.DWord);
        }

        private void disableAppUpdates_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Store", "AutoDownload", 4, RegistryValueKind.DWord);
        }

        // Gizlilik ve Güvenlik Regedit Kodları

        // Activity Feed
        private void activityFeed_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", 0, RegistryValueKind.DWord);
        }

        private void activityFeed_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", 1, RegistryValueKind.DWord);
        }

        // Telemetry
        private void telemetry_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
        }

        private void telemetry_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 1, RegistryValueKind.DWord);
        }

        // Cortana
        private void cortana_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
        }

        private void cortana_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 1, RegistryValueKind.DWord);
        }

        // Location
        private void location_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", 1, RegistryValueKind.DWord);
        }

        private void location_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", 0, RegistryValueKind.DWord);
        }

        // Advertising ID
        private void advertisingID_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", 1, RegistryValueKind.DWord);
        }

        private void advertisingID_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", 0, RegistryValueKind.DWord);
        }

        // Biometric
        private void biometric_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Biometrics", "Enabled", 0, RegistryValueKind.DWord);
        }

        private void biometric_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Biometrics", "Enabled", 1, RegistryValueKind.DWord);
        }

        // Windows Defender
        private void windowsDefender_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, RegistryValueKind.DWord);
        }

        private void windowsDefender_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 0, RegistryValueKind.DWord);
        }

        // OneDrive
        private void oneDrive_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 1, RegistryValueKind.DWord);
        }

        private void oneDrive_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 0, RegistryValueKind.DWord);
        }

        // Feedback Hub
        private void feedbackHub_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackTool", 1, RegistryValueKind.DWord);
        }

        private void feedbackHub_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "DoNotShowFeedbackTool", 0, RegistryValueKind.DWord);
        }

        // WiFi Sense
        private void wifiSense_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 0, RegistryValueKind.DWord);
        }

        private void wifiSense_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 1, RegistryValueKind.DWord);
        }

        // Diagnostic Data
        private void diagnosticData_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\SettingSync", "DisableSettingSync", 1, RegistryValueKind.DWord);
        }

        private void diagnosticData_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\SettingSync", "DisableSettingSync", 0, RegistryValueKind.DWord);
        }

        // Disable Diagnostics Tracking
        private void diagnosticsTrackingDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowDiagnostic", 0, RegistryValueKind.DWord);
        }

        private void diagnosticsTrackingDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowDiagnostic", 1, RegistryValueKind.DWord);
        }

        // SmartScreen
        private void smartScreen_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", 0, RegistryValueKind.DWord);
        }

        private void smartScreen_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", 1, RegistryValueKind.DWord);
        }

        // Location History
        private void locationHistory_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "LocationHistory", 0, RegistryValueKind.DWord);
        }

        private void locationHistory_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "LocationHistory", 1, RegistryValueKind.DWord);
        }

        // Xbox Game Bar
        private void xboxGameBar_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0, RegistryValueKind.DWord);
        }

        private void xboxGameBar_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 1, RegistryValueKind.DWord);
        }

        // Password Reveal Button
        private void passwordReveal_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\CredUI", "DisablePasswordReveal", 1, RegistryValueKind.DWord);
        }

        private void passwordReveal_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\CredUI", "DisablePasswordReveal", 0, RegistryValueKind.DWord);
        }

        // Location Sensors
        private void locationSensors_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableSensors", 1, RegistryValueKind.DWord);
        }

        private void locationSensors_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableSensors", 0, RegistryValueKind.DWord);
        }

        // WiFi Hotspot
        private void wifiHotspot_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Network Connections", "NC_ShowSharedAccessUI", 0, RegistryValueKind.DWord);
        }

        private void wifiHotspot_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Network Connections", "NC_ShowSharedAccessUI", 1, RegistryValueKind.DWord);
        }

        // Speech Recognition
        private void speechRecognition_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Speech", "AllowSpeechInput", 0, RegistryValueKind.DWord);
        }

        private void speechRecognition_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Speech", "AllowSpeechInput", 1, RegistryValueKind.DWord);
        }

        // Online Tips
        private void onlineTips_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\EdgeUI", "DisableHelpSticker", 1, RegistryValueKind.DWord);
        }

        private void onlineTips_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\EdgeUI", "DisableHelpSticker", 0, RegistryValueKind.DWord);
        }

        // App Suggestions
        private void appSuggestions_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsConsumerFeatures", 1, RegistryValueKind.DWord);
        }

        private void appSuggestions_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsConsumerFeatures", 0, RegistryValueKind.DWord);
        }

        // User Activity History
        private void userActivityHistory_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", 0, RegistryValueKind.DWord);
        }

        private void userActivityHistory_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities", 1, RegistryValueKind.DWord);
        }

        // WiFi Direct
        private void wifiDirect_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WcmSvc\GroupPolicy", "fMinimizeConnections", 0, RegistryValueKind.DWord);
        }

        private void wifiDirect_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\WcmSvc\GroupPolicy", "fMinimizeConnections", 1, RegistryValueKind.DWord);
        }

        // Application Telemetry
        private void appTelemetry_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", 0, RegistryValueKind.DWord);
        }

        private void appTelemetry_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\AppCompat", "AITEnable", 1, RegistryValueKind.DWord);
        }

        // Windows Troubleshooting
        private void troubleshooting_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);
        }

        private void troubleshooting_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Error Reporting", "Disabled", 0, RegistryValueKind.DWord);
        }

        // AutoPlay
        private void autoPlay_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "NoAutoplayfornonVolume", 1, RegistryValueKind.DWord);
        }

        private void autoPlay_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "NoAutoplayfornonVolume", 0, RegistryValueKind.DWord);
        }

        // Clipboard History
        private void clipboardHistory_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "AllowClipboardHistory", 0, RegistryValueKind.DWord);
        }

        private void clipboardHistory_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\System", "AllowClipboardHistory", 1, RegistryValueKind.DWord);
        }

        // Web Search
        private void webSearch_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
        }

        private void webSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 0, RegistryValueKind.DWord);
        }

        // Windows Spotlight
        private void windowsSpotlight_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsSpotlightFeatures", 1, RegistryValueKind.DWord);
        }

        private void windowsSpotlight_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\CloudContent", "DisableWindowsSpotlightFeatures", 0, RegistryValueKind.DWord);
        }

        // Windows Ink Workspace
        private void windowsInkWorkspace_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\WindowsInkWorkspace", "AllowSuggestedAppsInWindowsInkWorkspace", 0, RegistryValueKind.DWord);
        }

        private void windowsInkWorkspace_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\WindowsInkWorkspace", "AllowSuggestedAppsInWindowsInkWorkspace", 1, RegistryValueKind.DWord);
        }

        // Microsoft Edge Sync
        private void edgeSync_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Edge", "SyncDisabled", 1, RegistryValueKind.DWord);
        }

        private void edgeSync_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Edge", "SyncDisabled", 0, RegistryValueKind.DWord);
        }

        // Disable Error Reporting
        private void errorReportingDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);
        }

        private void errorReportingDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 0, RegistryValueKind.DWord);
        }

        // Disable Handwriting Data Sharing
        private void handwritingDataSharingDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", 1, RegistryValueKind.DWord);
        }

        private void handwritingDataSharingDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", 0, RegistryValueKind.DWord);
        }

        // Disable Cloud Content Suggestions
        private void cloudContentSuggestionsDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);
        }

        private void cloudContentSuggestionsDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 1, RegistryValueKind.DWord);
        }

        // Disable Online Tips and Help
        private void onlineTipsDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338388Enabled", 0, RegistryValueKind.DWord);
        }

        private void onlineTipsDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338388Enabled", 1, RegistryValueKind.DWord);
        }

        // Disable Windows Media DRM Internet Access
        private void mediaDRMInternetAccessDisable_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\WMDRM\NonSOSDevice", "DisableOnline", 1, RegistryValueKind.DWord);
        }

        private void mediaDRMInternetAccessDisable_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\WMDRM\NonSOSDevice", "DisableOnline", 0, RegistryValueKind.DWord);
        }

        // Add Ram Cleaner Shortcut


        private void ramCleaner_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Directory\Background\shell\ClearMemory", "", "Clear RAM", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Directory\Background\shell\ClearMemory", "Icon", "C:\\Windows\\ClearMemory.ico", RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.ClassesRoot, @"Directory\Background\shell\ClearMemory\Command", "", "C:\\Windows\\ClearMemory.bat", RegistryValueKind.String);
            MainLogic(); // Ana mantığı başlat
        }
        static void MainLogic()
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceDirectory = Path.Combine(appDirectory, "Resources"); // Kaynak dizin
            string destinationDirectory = @"C:\\Windows\\"; // Hedef dizin

            string[] filesToCopy = { "ClearMemory.bat", "ClearMemory.ico", "RAMMap.exe" };

            foreach (string fileName in filesToCopy)
            {
                string sourceFilePath = Path.Combine(sourceDirectory, fileName);
                string destinationFilePath = Path.Combine(destinationDirectory, fileName);

                File.Copy(sourceFilePath, destinationFilePath, true);
            }
        }

        private void ramCleaner_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.TryDeleteSubKeyTree(RegistryHive.ClassesRoot, @"Directory\Background\shell\ClearMemory");
            DelMainLogic();
        }
        static void DelMainLogic()
        {
            string appDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sourceDirectory = Path.Combine(appDirectory, "Resources"); // Kaynak dizin
            string destinationDirectory = @"C:\Windows\"; // Hedef dizin

            string[] filesToDelete = { "ClearMemory.bat", "ClearMemory.ico", "RAMMap.exe" };

            foreach (string fileName in filesToDelete)
            {
                string filePathToDelete = Path.Combine(destinationDirectory, fileName);

                if (File.Exists(filePathToDelete))
                {
                    File.Delete(filePathToDelete);
                    Console.WriteLine($"Deleted: {filePathToDelete}");
                }
                else
                {
                    Console.WriteLine($"File not found: {filePathToDelete}");
                }
            }
        }


        // Add Ram Cleaner Shortcut


        private void disableSearchIndex_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowSearchToUseLocation", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WSearch", "Start", 4, RegistryValueKind.DWord);

        }

        private void disableSearchIndex_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowSearchToUseLocation", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WSearch", "Start", 2, RegistryValueKind.DWord);
        }

        // AVCTP service
        private void AVCTP_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\BthAvctpSvc", "Start", 4, RegistryValueKind.DWord);
        }
        private void AVCTP_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WSearch", "Start", 3, RegistryValueKind.DWord);
        }

        // BitLocker Drive Encryption Service

        private void BitLockerDriveEncryptionService_Checked(object sender,RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\BDESVC", "Start", 4, RegistryValueKind.DWord);

        }
        private void BitLockerDriveEncryptionService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\BDESVC", "Start", 3, RegistryValueKind.DWord);
        }

        // Bluetooth Support Service

        private void BluetoothSupportService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\bthserv", "Start", 4, RegistryValueKind.DWord);
        }
        private void BluetoothSupportService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\bthserv", "Start", 3, RegistryValueKind.DWord);
        }

        // ConnectedUserExperiencesTelemetry

        private void ConnectedUserExperiencesTelemetry_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", 4, RegistryValueKind.DWord);

        }
        private void ConnectedUserExperiencesTelemetry_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", 2, RegistryValueKind.DWord);
        }

        // DiagnosticTrackingService

        private void DiagnosticTrackingService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", 4, RegistryValueKind.DWord);
        }
        private void DiagnosticTrackingService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", 2, RegistryValueKind.DWord);
        }

        // DownloadedMapsManager

        private void DownloadedMapsManager_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\MapsBroker", "Start", 4, RegistryValueKind.DWord);

        }
        private void DownloadedMapsManager_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\MapsBroker", "Start", 2, RegistryValueKind.DWord);
        }

        // FileHistoryService

        private void FileHistoryService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\fhsvc", "Start", 4, RegistryValueKind.DWord);

        }
        private void FileHistoryService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\fhsvc", "Start", 3, RegistryValueKind.DWord);
        }

        // IPHelper

        private void IPHelper_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\iphlpsvc", "Start", 4, RegistryValueKind.DWord);
        }
        private void IPHelper_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\iphlpsvc", "Start", 2, RegistryValueKind.DWord);
        }

        // Windows Update Medic Service
        private void WindowsUpdateMedicService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\waauserv", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsUpdateMedicService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\waauserv", "Start", 2, RegistryValueKind.DWord);
        }

        // Windows Biometric Service
        private void WindowsBiometricService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WbioSrvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsBiometricService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WbioSrvc", "Start", 3, RegistryValueKind.DWord);
        }

        // Windows Time Service
        private void WindowsTimeService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\W32Time", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsTimeService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\W32Time", "Start", 3, RegistryValueKind.DWord);
        }

        // Windows Modules Installer
        private void WindowsModulesInstaller_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TrustedInstaller", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsModulesInstaller_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TrustedInstaller", "Start", 3, RegistryValueKind.DWord);
        }

        // Windows Remote Management (WinRM) Service
        private void WinRMServer_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WinRM", "Start", 4, RegistryValueKind.DWord);
        }

        private void WinRMServer_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WinRM", "Start", 3, RegistryValueKind.DWord);
        }

        // Windows Event Log Service
        private void EventLogService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\EventLog", "Start", 4, RegistryValueKind.DWord);
        }

        private void EventLogService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\EventLog", "Start", 3, RegistryValueKind.DWord);
        }


        // Windows Insider Service
        private void WindowsInsiderService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WaaSMedicSvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsInsiderService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WaaSMedicSvc", "Start", 3, RegistryValueKind.DWord);
        }

        // Remote Desktop Services
        private void RemoteDesktopServices_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TermService", "Start", 4, RegistryValueKind.DWord);
        }

        private void RemoteDesktopServices_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TermService", "Start", 3, RegistryValueKind.DWord);
        }

        // Print Spooler Service
        private void PrintSpoolerService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Spooler", "Start", 4, RegistryValueKind.DWord);
        }

        private void PrintSpoolerService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\Spooler", "Start", 2, RegistryValueKind.DWord);
        }

        // Security Center Service
        private void SecurityCenterService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wscsvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void SecurityCenterService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wscsvc", "Start", 2, RegistryValueKind.DWord);
        }

        // Windows Error Reporting Service
        private void WindowsErrorReportingService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WerSvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsErrorReportingService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WerSvc", "Start", 2, RegistryValueKind.DWord);
        }

        // Windows Media Player Network Sharing Service
        private void WMPNetworkSharingService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WMPNetworkSvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void WMPNetworkSharingService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WMPNetworkSvc", "Start", 3, RegistryValueKind.DWord);
        }

        // WebClient Service
        private void WebClientService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WebClient", "Start", 2, RegistryValueKind.DWord);
        }

        private void WebClientService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WebClient", "Start", 4, RegistryValueKind.DWord);
        }

        // Superfetch Service
        private void SuperfetchService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 2, RegistryValueKind.DWord);
        }

        private void SuperfetchService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 4, RegistryValueKind.DWord);
        }

        // Remote Registry Service
        private void RemoteRegistryService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\RemoteRegistry", "Start", 2, RegistryValueKind.DWord);
        }

        private void RemoteRegistryService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\RemoteRegistry", "Start", 4, RegistryValueKind.DWord);
        }

        // Background Intelligent Transfer Service
        private void BITS_Service_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\BITS", "Start", 2, RegistryValueKind.DWord);
        }

        private void BITS_Service_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\BITS", "Start", 3, RegistryValueKind.DWord);
        }

        // Data Usage Service
        private void DataUsageService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DusmSvc", "Start", 2, RegistryValueKind.DWord);
        }

        private void DataUsageService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\DusmSvc", "Start", 3, RegistryValueKind.DWord);
        }

        // Geolocation Service
        private void GeolocationService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\lfsvc", "Start", 2, RegistryValueKind.DWord);
        }

        private void GeolocationService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\lfsvc", "Start", 3, RegistryValueKind.DWord);
        }

        // Credential Manager Service
        private void CredentialManagerService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\VaultSvc", "Start", 2, RegistryValueKind.DWord);
        }

        private void CredentialManagerService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\VaultSvc", "Start", 3, RegistryValueKind.DWord);
        }

        // Distributed Link Tracking Client
        private void DistributedLinkTrackingClient_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TrkWks", "Start", 2, RegistryValueKind.DWord);
        }

        private void DistributedLinkTrackingClient_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\TrkWks", "Start", 3, RegistryValueKind.DWord);
        }

        // SysMain Service
        private void SysMainService_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 2, RegistryValueKind.DWord);
        }

        private void SysMainService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\SysMain", "Start", 3, RegistryValueKind.DWord);
        }

        // Disable Camera Access
        private void CameraAccess_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam", "Value", "Deny", RegistryValueKind.String);
        }

        private void CameraAccess_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam", "Value", "Allow", RegistryValueKind.String);
        }

        // Disable Microphone Access
        private void MicrophoneAccess_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone", "Value", "Deny", RegistryValueKind.String);
        }

        private void MicrophoneAccess_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone", "Value", "Allow", RegistryValueKind.String);
        }

        // Disable App Notifications
        private void AppNotifications_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", 1, RegistryValueKind.DWord);
        }

        private void AppNotifications_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", 0, RegistryValueKind.DWord);
        }

        // Disable Sync with Devices
        private void SyncWithDevices_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 1, RegistryValueKind.DWord);
        }

        private void SyncWithDevices_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 0, RegistryValueKind.DWord);
        }

        // Disable App Access to Account Info
        private void AppAccountInfoAccess_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\default\Settings\AllowMicrosoftAccountToSync", "Value", 0, RegistryValueKind.DWord);
        }

        private void AppAccountInfoAccess_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\PolicyManager\default\Settings\AllowMicrosoftAccountToSync", "Value", 1, RegistryValueKind.DWord);
        }

        // Disable Cortana Web Search
        private void CortanaWebSearch_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
        }

        private void CortanaWebSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 1, RegistryValueKind.DWord);
        }

        // Disable AutoRun for Removable Media
        private void AutoRunRemovableMedia_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 255, RegistryValueKind.DWord);
        }

        private void AutoRunRemovableMedia_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoDriveTypeAutoRun", 145, RegistryValueKind.DWord);
        }

        // Disable Lock Screen Camera
        private void LockScreenCamera_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreenCamera", 1, RegistryValueKind.DWord);
        }

        private void LockScreenCamera_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreenCamera", 0, RegistryValueKind.DWord);
        }

        // Disable Windows Game Recording and Broadcasting
        private void GameRecording_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0, RegistryValueKind.DWord);
        }

        private void GameRecording_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 1, RegistryValueKind.DWord);

        }


        // Old Photo Viewer
        private void oldPhotoViewer_Checked(object sender, RoutedEventArgs e)
        {
            // Önce çalışma dizininin yolunu alalım
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Resources klasörü içindeki "photo-viewer.ps1" dosyasının tam yolunu oluşturalım
            string ps1FilePath = Path.Combine(currentDirectory, "Resources", "photo-viewer.ps1");

            // Eğer kullanıcı yönetici yetkisine sahipse, PowerShell betiğini gizli olarak çalıştıralım
            if (IsAdministrator())
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -WindowStyle Hidden -File \"{ps1FilePath}\"",
                    Verb = "runas"  // Yönetici yetkisiyle çalıştırma için
                };

                try
                {
                    Process.Start(psi);
                }
                catch (Exception)
                {
                }
            }
            else
            {
            }
        }

        // Kullanıcının yönetici yetkisine sahip olup olmadığını kontrol eden fonksiyon
        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        private void oldPhotoViewer_Unchecked(object sender, RoutedEventArgs e)
        {

        }


        private bool isProcessing = false;
        private bool showCleaningCompleteMessage = false;

        private string FormatSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return String.Format("{0:0.##} {1}", size, sizes[order]);
        }

        private async Task DeleteFilesAndFoldersAsync(params string[] paths)
        {
            isProcessing = true;

            foreach (string path in paths)
            {
                await DeletePathAsync(path);
            }

            isProcessing = false;

            if (!showCleaningCompleteMessage)
            {
                outputTextBox.Text = string.Empty;
                MessageBoxShown = false;
                System.Windows.MessageBox.Show("Cleaning process completed.", "Process Completed", MessageBoxButton.OK, MessageBoxImage.Information);

            }

            sizeLabel.Content = "Size : 0 B";
        }
        private bool MessageBoxShown = false; // MessageBox gösterildi mi?

        private async Task DeletePathAsync(string path)
        {
            bool deletionErrorOccurred = false; // Hata durumunu kontrol etmek için bir bayrak

            try
            {
                if (File.Exists(path))
                {
                    await Task.Run(() =>
                    {
                        TakeOwnership(path);
                        File.Delete(path);
                    });
                }
                else if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path);
                    string[] folders = Directory.GetDirectories(path);

                    foreach (string file in files)
                    {
                        await DeletePathAsync(file);
                    }

                    foreach (string folder in folders)
                    {
                        await DeletePathAsync(folder);
                    }

                    await Task.Run(() =>
                    {
                        Directory.Delete(path, true);
                    });
                }
            }
            catch (UnauthorizedAccessException)
            {
                deletionErrorOccurred = true; // Hata durumu olduğunu işaretle
            }
            catch (IOException)
            {
                deletionErrorOccurred = true; // Hata durumu olduğunu işaretle
            }
            catch (Exception)
            {
                deletionErrorOccurred = true; // Hata durumu olduğunu işaretle
            }

            // Tüm işlem bittikten sonra, eğer hata durumu varsa ve daha önce hata gösterilmediyse MessageBox göster
            if (deletionErrorOccurred)
            {
                if (!MessageBoxShown)
                {
                    System.Windows.MessageBox.Show("Bazı dosyalar ve klasörler arkaplanda açık uygulamalar tarafından kullanılıyor, bu dosya ve klasörler silinemeyecek!.");
                    MessageBoxShown = true; // MessageBox gösterildiğini işaretle
                }
            }
        }




        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            sizeLabel.Content = "Size : 0 B";

            if (!isProcessing)
            {
                long totalSize = 0;
                List<string> selectedPaths = new List<string>();
                progressBar.IsIndeterminate = true;
                isProcessing = true;
                await Task.Delay(3000);

                if (LogsCheckbox.IsChecked == true)
                {
                    string updateLogsPath = @"C:\Windows\Logs\";
                    if (Directory.Exists(updateLogsPath))
                    {
                        selectedPaths.Add(updateLogsPath);

                        string[] logFiles = Directory.GetFiles(updateLogsPath, "*", SearchOption.AllDirectories);

                        foreach (string file in logFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (tempCheckbox.IsChecked == true)
                {
                    string username = Environment.UserName;
                    string tempPath = Path.Combine(@"C:\Users", username, "AppData", "Local", "Temp");
                    if (Directory.Exists(tempPath))
                    {
                        selectedPaths.Add(tempPath);

                        string[] tempFiles = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);

                        foreach (string file in tempFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (WindowsOldCheckbox.IsChecked == true)
                {
                    string windowsOldPath = @"C:\Windows.old\";
                    if (Directory.Exists(windowsOldPath))
                    {
                        selectedPaths.Add(windowsOldPath);

                        string[] windowsOldFiles = Directory.GetFiles(windowsOldPath, "*", SearchOption.AllDirectories);

                        foreach (string file in windowsOldFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (erroreportingCheckbox.IsChecked == true)
                {
                    string werPath = @"C:\ProgramData\Microsoft\Windows\WER\";
                    if (Directory.Exists(werPath))
                    {
                        selectedPaths.Add(werPath);

                        string[] werFiles = Directory.GetFiles(werPath, "*", SearchOption.AllDirectories);

                        foreach (string file in werFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (programdatacachesCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\ProgramData\Microsoft\Windows\Caches\";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (downloadsCheckbox.IsChecked == true)
                {
                    string username = Environment.UserName;
                    string tempPath = Path.Combine(@"C:\Users", username, "Downloads");
                    if (Directory.Exists(tempPath))
                    {
                        selectedPaths.Add(tempPath);

                        string[] tempFiles = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);

                        foreach (string file in tempFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (recyclebinCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\$Recycle.Bin";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (searchindexCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\ProgramData\Microsoft\Search\Data\";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (prefetchCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\Windows\Prefetch\";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (fontcacheCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\Windows\ServiceProfiles\LocalService\AppData\Local\FontCache";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                if (ınstallerCheckbox.IsChecked == true)
                {
                    string cachesPath = @"C:\Windows\Installer";
                    if (Directory.Exists(cachesPath))
                    {
                        selectedPaths.Add(cachesPath);

                        string[] cacheFiles = Directory.GetFiles(cachesPath, "*", SearchOption.AllDirectories);

                        foreach (string file in cacheFiles)
                        {
                            totalSize += new FileInfo(file).Length;
                        }
                    }
                }

                // Diğer checkbox'lar için benzer mantık

                List<string> allFiles = new List<string>();
                foreach (string path in selectedPaths)
                {
                    allFiles.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                }

                outputTextBox.Text = string.Join(Environment.NewLine, allFiles);
                sizeLabel.Content = "Size : " + FormatSize(totalSize);
                progressBar.IsIndeterminate = false;
                isProcessing = false;
            }
        }


        private async void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isProcessing) // Check if not already processing
            {
                isProcessing = true;
                string[] selectedPath = new string[11];

                /// CLEAR  BAŞLANGICI ///

                if (LogsCheckbox.IsChecked == true)
                {
                    selectedPath[0] = @"C:\Windows\Logs\";
                }

                if (tempCheckbox.IsChecked == true)
                {
                    string username = Environment.UserName;
                    selectedPath[1] = Path.Combine(@"C:\Users", username, "AppData", "Local", "Temp");
                }

                if (WindowsOldCheckbox.IsChecked == true)
                {
                    selectedPath[2] = Path.Combine(@"C:\Windows.old");
                }

                if (erroreportingCheckbox.IsChecked == true)
                {
                    selectedPath[3] = Path.Combine(@"C:\ProgramData\Microsoft\Windows\WER");
                }

                if (programdatacachesCheckbox.IsChecked == true)
                {
                    selectedPath[4] = Path.Combine(@"C:\ProgramData\Microsoft\Windows\Caches");
                }

                if (downloadsCheckbox.IsChecked == true)
                {
                    string username = Environment.UserName;
                    selectedPath[5] = Path.Combine(@"C:\Users", username, "Downloads");
                }

                if (recyclebinCheckbox.IsChecked == true)
                {
                    selectedPath[6] = Path.Combine(@"C:\$Recycle.Bin");
                }

                if (searchindexCheckbox.IsChecked == true)
                {
                    selectedPath[7] = Path.Combine(@"C:\ProgramData\Microsoft\Search\Data");
                }

                if (prefetchCheckbox.IsChecked == true)
                {
                    selectedPath[8] = Path.Combine(@"C:\Windows\Prefetch");
                }

                if (fontcacheCheckbox.IsChecked == true)
                {
                    selectedPath[9] = Path.Combine(@"C:\Windows\ServiceProfiles\LocalService\AppData\Local\FontCache");
                }

                if (ınstallerCheckbox.IsChecked == true)
                {
                    selectedPath[10] = Path.Combine(@"C:\Windows\Installer");
                }
                // ... (Diğer checkbox'lar için benzer mantık)
                await DeleteFilesAndFoldersAsync(selectedPath);
                sizeLabel.Content = "Size : 0 B";
                isProcessing = false;
            }
        }

        private void TakeOwnership(string filePath)
        {
            try
            {
                System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.AccessControl.FileSecurity fSecurity = File.GetAccessControl(filePath);
                fSecurity.SetOwner(identity.User);
                File.SetAccessControl(filePath, fSecurity);
            }
            catch (Exception)
            {
            }
        }

        private void comboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            string basePath = AppDomain.CurrentDomain.BaseDirectory; // Programın çalıştığı dizin

            if (comboBoxLanguage.SelectedIndex == 0)
            {
                resourceDictionary.Source = new Uri(Path.Combine(basePath, "StringResources.en.xaml"));
            }
            else if (comboBoxLanguage.SelectedIndex == 1)
            {
                resourceDictionary.Source = new Uri(Path.Combine(basePath, "StringResources.tr.xaml"));
            }
            else if (comboBoxLanguage.SelectedIndex == 2)
            {
                resourceDictionary.Source = new Uri(Path.Combine(basePath, "StringResources.de.xaml"));
            }
            else if (comboBoxLanguage.SelectedIndex == 3)
            {
                resourceDictionary.Source = new Uri(Path.Combine(basePath, "StringResources.es.xaml"));
            }
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }





    }
}