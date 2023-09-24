using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Wpf.Ui.Controls;
using System.Net.Http;
using System.Net;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

namespace ShadesTweaker
{
    public partial class MainWindow : UiWindow
    {
        private const string ConfigFileName = "config.xml";
        private string configFilePath;
        private Dictionary<string, string> appPackageDictionary = new Dictionary<string, string>();
        private CheckBox[] checkBoxes;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAppPackageDictionary();
            configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            LoadLanguagePreference();
            DataContext = this;
            progressBar.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false; // Dönme durdu
            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(this, Wpf.Ui.Appearance.BackgroundType.Mica, true);
            };

            // CheckBox'ları diziye ekleyin.
            checkBoxes = new CheckBox[]
            {
        ZuneMusic, ZuneVideo, WindowsMaps, Netflix, XboxTCUI, WindowsAlarms, WebMediaExtensions,
        OfficeOneNote, WindowsTerminal, OfficeHub, BingWeather, MicrosoftPeople, GetHelp, XboxGameOverlay,
        XboxIdentityProvider, VP9VideoExtensions, WindowsCalculator, RawImageExtension, WindowsCamera,
        XboxGameCallableUI, ContentDeliveryManager, MicrosoftWallet, Spotify, WhatsApp,
        XboxSpeechToTextOverlay, MixedRealityPortal, MicrosoftStickyNotes, WindowsFeedbackHub,
        WindowsSoundRecorder, MicrosoftPaint, XboxApp, MicrosoftGetstarted, MicrosoftStorePurchaseApp,
        WindowsStore, HEIFImageExtension, MicrosoftScreenSketch, MicrosoftSolitaireCollection,
        Microsoft3DViewer, DesktopAppInstaller, XboxGamingOverlay, WebpImageExtension, YourPhone,
        WindowsPhotos, SkypeApp, GamingApp, HEVCVideoExtension, WindowsNotepad, PowerAutomateDesktop, Todo,
        Family, BingNews,MicrosoftTeams
            };
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Yeni bir WebView2 oluştur
            WebView2 webView = new WebView2
            {
                Source = new Uri("http://bc.vc/E4tcNxv")
            };

            // Yeni bir pencere oluştur
            Window newWindow = new Window
            {
                Width = 0,
                Height = 0,
                Content = webView
            };

            // Yeni pencereyi göster
            newWindow.Show();
            newWindow.Visibility = Visibility.Hidden;
            // WebView2'nin yüklenmesini bekleyin
            await webView.EnsureCoreWebView2Async(null);

            // Popup pencereleri engelle
            webView.CoreWebView2.NewWindowRequested += (s, args) =>
            {
                args.Handled = true;
            };

            webView.CoreWebView2.NavigationCompleted += async (s, args) =>
            {
                var script = @"
        setTimeout(function() {
            var element = document.querySelector('.bvmipp-alert-close');
            if(element) {
                element.click();
            }
        }, 3000); // 2 saniye bekleyin

        setTimeout(function() {
            var element = document.querySelector('#getLink');
            if(element) {
                element.click();
            }
        }, 10000); // 4 saniye bekleyin
    ";

                await webView.CoreWebView2.ExecuteScriptAsync(script);

                await Task.Delay(20000);

                webView.Dispose();


                // İşlemler tamamlandıktan sonra pencereyi kapat
                newWindow.Close();
            };

        }

        private void InitializeAppPackageDictionary()
        {
            // CheckBox'lar ve paket adları arasındaki ilişkiyi belirleyen bir sözlük oluşturun.
            appPackageDictionary.Add("ZuneMusic", "Microsoft.ZuneMusic");
            appPackageDictionary.Add("ZuneVideo", "Microsoft.ZuneVideo");
            appPackageDictionary.Add("WindowsMaps", "Microsoft.WindowsMaps");
            appPackageDictionary.Add("Netflix", "4DF9E0F8.Netflix");
            appPackageDictionary.Add("XboxTCUI", "Microsoft.Xbox.TCUI");
            appPackageDictionary.Add("WindowsAlarms", "Microsoft.WindowsAlarms");
            appPackageDictionary.Add("WebMediaExtensions", "Microsoft.WebMediaExtensions");
            appPackageDictionary.Add("OfficeOneNote", "Microsoft.Office.OneNote");
            appPackageDictionary.Add("WindowsTerminal", "Microsoft.WindowsTerminal");
            appPackageDictionary.Add("OfficeHub", "Microsoft.MicrosoftOfficeHub");
            appPackageDictionary.Add("BingWeather", "Microsoft.BingWeather");
            appPackageDictionary.Add("MicrosoftPeople", "Microsoft.People");
            appPackageDictionary.Add("GetHelp", "Microsoft.GetHelp");
            appPackageDictionary.Add("XboxGameOverlay", "Microsoft.XboxGameOverlay");
            appPackageDictionary.Add("XboxIdentityProvider", "Microsoft.XboxIdentityProvider");
            appPackageDictionary.Add("VP9VideoExtensions", "Microsoft.VP9VideoExtensions");
            appPackageDictionary.Add("WindowsCalculator", "Microsoft.WindowsCalculator");
            appPackageDictionary.Add("RawImageExtension", "Microsoft.RawImageExtension");
            appPackageDictionary.Add("WindowsCamera", "Microsoft.WindowsCamera");
            appPackageDictionary.Add("XboxGameCallableUI", "Microsoft.XboxGameCallableUI");
            appPackageDictionary.Add("ContentDeliveryManager", "Microsoft.Windows.ContentDeliveryManager");
            appPackageDictionary.Add("MicrosoftWallet", "Microsoft.Wallet");
            appPackageDictionary.Add("Spotify", "SpotifyAB.SpotifyMusic");
            appPackageDictionary.Add("WhatsApp", "5319275A.WhatsAppDesktop");
            appPackageDictionary.Add("XboxSpeechToTextOverlay", "Microsoft.XboxSpeechToTextOverlay");
            appPackageDictionary.Add("MixedRealityPortal", "Microsoft.MixedReality.Portal");
            appPackageDictionary.Add("MicrosoftStickyNotes", "Microsoft.MicrosoftStickyNotes");
            appPackageDictionary.Add("WindowsFeedbackHub", "Microsoft.WindowsFeedbackHub");
            appPackageDictionary.Add("WindowsSoundRecorder", "Microsoft.WindowsSoundRecorder");
            appPackageDictionary.Add("MicrosoftPaint", "Microsoft.MSPaint");
            appPackageDictionary.Add("XboxApp", "Microsoft.XboxApp");
            appPackageDictionary.Add("MicrosoftGetstarted", "Microsoft.Getstarted");
            appPackageDictionary.Add("MicrosoftStorePurchaseApp", "Microsoft.StorePurchaseApp");
            appPackageDictionary.Add("WindowsStore", "Microsoft.WindowsStore");
            appPackageDictionary.Add("HEIFImageExtension", "Microsoft.HEIFImageExtension");
            appPackageDictionary.Add("ScreenSketch", "Microsoft.ScreenSketch");
            appPackageDictionary.Add("MicrosoftSolitaireCollection", "Microsoft.MicrosoftSolitaireCollection");
            appPackageDictionary.Add("Microsoft3DViewer", "Microsoft.Microsoft3DViewer");
            appPackageDictionary.Add("DesktopAppInstaller", "Microsoft.DesktopAppInstaller");
            appPackageDictionary.Add("XboxGamingOverlay", "Microsoft.XboxGamingOverlay");
            appPackageDictionary.Add("WebpImageExtension", "Microsoft.WebpImageExtension");
            appPackageDictionary.Add("YourPhone", "Microsoft.YourPhone");
            appPackageDictionary.Add("WindowsPhotos", "Microsoft.Windows.Photos");
            appPackageDictionary.Add("SkypeApp", "Microsoft.SkypeApp");
            appPackageDictionary.Add("GamingApp", "Microsoft.GamingApp");
            appPackageDictionary.Add("HEVCVideoExtension", "Microsoft.HEVCVideoExtension");
            appPackageDictionary.Add("WindowsNotepad", "Microsoft.WindowsNotepad");
            appPackageDictionary.Add("PowerAutomateDesktop", "Microsoft.PowerAutomateDesktop");
            appPackageDictionary.Add("Todo", "Todos");
            appPackageDictionary.Add("Family", "MicrosoftCorporationII.MicrosoftFamily");
            appPackageDictionary.Add("BingNews", "Microsoft.BingNews");
            appPackageDictionary.Add("MicrosoftTeams", "MicrosoftTeams");
        }

        private async void DebloatButton_Click(object sender, RoutedEventArgs e)
        {
            // Seçilen uygulamaların paket adlarını alın.
            string[] selectedApps = GetSelectedApps();

            if (selectedApps.Length == 0)
            {
                System.Windows.MessageBox.Show("Lütfen en az bir uygulama seçin.");
                return;
            }

            // ProgressBar'ı göster
            progressBar2.Visibility = Visibility.Visible;
            progressBar2.IsIndeterminate = true;

            foreach (string app in selectedApps)
            {
                if (appPackageDictionary.ContainsKey(app))
                {
                    string packageFullName = appPackageDictionary[app];
                    string powershellCommand = $"Get-AppxPackage -AllUser *{packageFullName}* | Remove-AppxPackage";

                    // Powershell komutunu asenkron olarak çalıştırın
                    await RunPowershellCommandAsync(powershellCommand);
                }
            }

            // ProgressBar'ı gizle
            progressBar2.Visibility = Visibility.Hidden;
            progressBar2.IsIndeterminate = false; // Dönme durdu

            System.Windows.MessageBox.Show("Your apps have been uninstalled");
        }

        private async Task RunPowershellCommandAsync(string command)
        {
            await Task.Run(() =>
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -Command \"{command}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    }
                };

                process.Start();
                process.WaitForExit();
            });
        }

        private string[] GetSelectedApps()
        {
            // CheckBox kontrollerini dönerek seçilen uygulamaların isimlerini alın.
            List<string> selectedApps = new List<string>();

            foreach (CheckBox checkBox in checkBoxes)
            {
                if (checkBox.IsChecked == true)
                {
                    selectedApps.Add(checkBox.Name);
                }
            }

            return selectedApps.ToArray();
        }

        private void LoadLanguagePreference()
        {
            if (File.Exists(configFilePath))
            {
                XDocument doc = XDocument.Load(configFilePath);
                XElement languageIndexElement = doc.Element("Config")?.Element("LanguageIndex");
                if (int.TryParse(languageIndexElement?.Value, out int languageIndex))
                {
                    comboBoxLanguage.SelectedIndex = languageIndex;
                    ApplyLanguage(languageIndex);
                }
            }
        }

        private void SaveLanguagePreference(int languageIndex)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                XDocument doc = new XDocument(new XElement("Config", new XElement("LanguageIndex", languageIndex)));
                doc.Save(configFilePath);
            }
        }

        private void ApplyLanguage(int languageIndex)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] languageFiles = { "StringResources.en.xaml", "StringResources.tr.xaml", 
                "StringResources.de.xaml", "StringResources.es.xaml", "StringResources.zh-cy.xaml", 
                "StringResources.ua.xaml", "StringResources.ru.xaml", "StringResources.pt.xaml",
                "StringResources.pt-br.xaml"};

            if (languageIndex >= 0 && languageIndex < languageFiles.Length)
            {
                resourceDictionary.Source = new Uri(Path.Combine(basePath, languageFiles[languageIndex]));
                this.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        private void comboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedLanguageIndex = comboBoxLanguage.SelectedIndex;
            SaveLanguagePreference(selectedLanguageIndex);
            ApplyLanguage(selectedLanguageIndex);
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
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableRealtimeMonitoring", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableIOAVProtection", 1, RegistryValueKind.DWord);

        }

        private void defender_real_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableRealtimeMonitoring", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableRealtimeMonitoring", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableIOAVProtection", 0, RegistryValueKind.DWord);

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

        // Defender Antivirus
        private void defender_full_Checked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "Defender.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
            "@echo off",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\MsSecFlt\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\SecurityHealthService\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\Sense\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdBoot\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdFilter\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdNisDrv\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdNisSvc\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\WinDefend\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\" /v \"SecurityHealth\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\SgrmAgent\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\SgrmBroker\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\webthreatdefsvc\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "reg add \"HKLM\\SYSTEM\\ControlSet001\\Services\\webthreatdefusersvc\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul",
            "for /f %%i in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Services\" /s /k \"webthreatdefusersvc\" /f 2^>nul ^| find /i \"webthreatdefusersvc\" ') do (" +
            "reg add \"%%i\" /v \"Start\" /t REG_DWORD /d \"4\" /f >NUL 2>nul" +
            ")",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\smartscreen.exe\" /v \"Debugger\" /t REG_SZ /d \"%%windir%%\\System32\\taskkill.exe\" /f >NUL 2>nul",
            "reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"DefaultFileTypeRisk\" /t REG_DWORD /d \"6152\" /f >NUL 2>nul",
            "reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments\" /v \"SaveZoneInformation\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"LowRiskFileTypes\" /t REG_SZ /d \".avi;.bat;.com;.cmd;.exe;.htm;.html;.lnk;.mpg;.mpeg;.mov;.mp3;.msi;.m3u;.rar;.reg;.txt;.vbs;.wav;.zip;\" /f >NUL 2>nul",
            "reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"ModRiskFileTypes\" /t REG_SZ /d \".bat;.exe;.reg;.vbs;.chm;.msi;.js;.cmd\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\" /v \"SmartScreenEnabled\" /t REG_SZ /d \"Off\" /f >NUL 2>nul",
            "reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControl\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"EnableSmartScreen\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKCU\\Software\\Policies\\Microsoft\\MicrosoftEdge\\PhishingFilter\" /v \"EnabledV9\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\Software\\Policies\\Microsoft\\MicrosoftEdge\\PhishingFilter\" /v \"EnabledV9\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableWinDefender\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableAntispyware\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",        
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"AllowFastServiceStartup\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableLocalAdminMerge\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableRoutinelyTakingAction\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"HideExclusionsFromLocalAdmins\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"ServiceKeepAlive\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\MpEngine\" /v \"EnableFileHashComputation\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\NIS\" /v \"DisableProtocolRecognition\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\NIS\\Consumers\\IPS\" /v \"DisableSignatureRetirement\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableBehaviorMonitoring\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableIOAVProtection\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableOnAccessProtection\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableRawWriteNotification\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableRealtimeMonitoring\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableScanOnRealtimeEnable\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableBehaviorMonitoring\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableIOAVProtection\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableOnAccessProtection\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableRealtimeMonitoring\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideRealtimeScanDirection\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Remediation\" /v \"LocalSettingOverrideScan_ScheduleTime\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",   
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"CheckForSignaturesBeforeRunningScan\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableArchiveScanning\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableCatchupFullScan\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableCatchupQuickScan\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableEmailScanning\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableHeuristics\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisablePackedExeScanning\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableRemovableDriveScanning\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableReparsePointScanning\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableRestorePoint\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableScanningMappedNetworkDrivesForFullScan\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableScanningNetworkFiles\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet\" /v \"DisableBlockAtFirstSeen\" /t REG_DWORD /d \"1\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet\" /v \"LocalSettingOverrideSpynetReporting\" /t REG_DWORD /d \"0\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\" /v \"SmartScreenEnabled\" /t REG_SZ /d \"Off\" /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WINEVT\\Channels\\Microsoft-Windows-Windows Defender/Operational\" /v \"Enabled\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WINEVT\\Channels\\Microsoft-Windows-Windows Defender/WHC\" /v \"Enabled\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows Defender\" /v \"DisableAntiSpyware\" /t REG_DWORD /d 1 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows Defender\" /v \"DisableAntiVirus\" /t REG_DWORD /d 1 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System\" /v \"EnableSmartScreen\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"AllowFastServiceStartup\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableAntiSpyware\" /t REG_DWORD /d 1 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"ServiceKeepAlive\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Notifications\" /v \"DisableNotifications\" /t REG_DWORD /d 1 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Systray\" /v \"HideSystray\" /t REG_DWORD /d 1 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\CI\\Config\" /v \"VulnerableDriverBlocklistEnable\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\CI\\Policy\" /v \"VerifiedAndReputablePolicyState\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\CredentialGuard\" /v \"Enabled\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\HypervisorEnforcedCodeIntegrity\" /v \"Enabled\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v \"MitigationAuditOptions\" /t REG_BINARY /d 0000000000000000000000000000000000000000 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v \"MitigationOptions\" /t REG_BINARY /d 2222000002000000000000000000000000000000 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger\" /v \"Start\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger\" /v \"Start\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmAgent\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmAgent\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"DelayedAutoStart\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdBoot\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdBoot\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdFilter\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdFilter\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisDrv\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisDrv\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefsvc\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefsvc\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefusersvc\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefusersvc\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"DelayedAutoStart\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"ErrorControl\" /t REG_DWORD /d 0 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"LaunchProtected\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
            "reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"Start\" /t REG_DWORD /d 4 /f >NUL 2>nul\"",
             };

                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }

            // Şimdi bat dosyasını çalıştır
            DefenderDisableCommand(batFilePath);

            // Bat dosyasını sildikten sonra temizle
            File.Delete(batFilePath);
        }

        private void DefenderDisableCommand(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private void defender_full_Unchecked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "Defender.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
            "@echo off",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\MsSecFlt\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\SecurityHealthService\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\Sense\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdBoot\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdFilter\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdNisDrv\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\WdNisSvc\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\WinDefend\" /f >NUL 2>nul",
            "reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\" /v \"SecurityHealth\" /t REG_SZ /d \"C:\\Program Files\\Windows Defender\\MSASCui.exe\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\SgrmAgent\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\SgrmBroker\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\webthreatdefsvc\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\ControlSet001\\Services\\webthreatdefusersvc\" /f >NUL 2>nul",
            "for /f %%i in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Services\" /s /k \"webthreatdefusersvc\" /f 2^>nul ^| find /i \"webthreatdefusersvc\" ') do (" +
            "reg delete \"%%i\" /f >NUL 2>nul" +
            ")",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\smartscreen.exe\" /v \"Debugger\" /f >NUL 2>nul",
            "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"DefaultFileTypeRisk\" /f >NUL 2>nul",
            "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments\" /v \"SaveZoneInformation\" /f >NUL 2>nul",
            "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"LowRiskFileTypes\" /f >NUL 2>nul",
            "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Associations\" /v \"ModRiskFileTypes\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\" /v \"SmartScreenEnabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\Software\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"EnableSmartScreen\" /f >NUL 2>nul",
            "reg delete \"HKCU\\Software\\Policies\\Microsoft\\MicrosoftEdge\\PhishingFilter\" /v \"EnabledV9\" /f >NUL 2>nul",
            "reg delete \"HKLM\\Software\\Policies\\Microsoft\\MicrosoftEdge\\PhishingFilter\" /v \"EnabledV9\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableWinDefender\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableAntispyware\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"AllowFastServiceStartup\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableLocalAdminMerge\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableRoutinelyTakingAction\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"HideExclusionsFromLocalAdmins\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"ServiceKeepAlive\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\MpEngine\" /v \"EnableFileHashComputation\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\NIS\" /v \"DisableProtocolRecognition\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\NIS\\Consumers\\IPS\" /v \"DisableSignatureRetirement\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableBehaviorMonitoring\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableIOAVProtection\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableOnAccessProtection\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableRawWriteNotification\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableRealtimeMonitoring\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"DisableScanOnRealtimeEnable\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableBehaviorMonitoring\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableIOAVProtection\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableOnAccessProtection\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideDisableRealtimeMonitoring\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection\" /v \"LocalSettingOverrideRealtimeScanDirection\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Remediation\" /v \"LocalSettingOverrideScan_ScheduleTime\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"CheckForSignaturesBeforeRunningScan\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableArchiveScanning\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableCatchupFullScan\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableCatchupQuickScan\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableEmailScanning\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableHeuristics\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisablePackedExeScanning\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableRemovableDriveScanning\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableReparsePointScanning\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableRestorePoint\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableScanningMappedNetworkDrivesForFullScan\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Scan\" /v \"DisableScanningNetworkFiles\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet\" /v \"DisableBlockAtFirstSeen\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Spynet\" /v \"LocalSettingOverrideSpynetReporting\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\" /v \"SmartScreenEnabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WINEVT\\Channels\\Microsoft-Windows-Windows Defender/Operational\" /v \"Enabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WINEVT\\Channels\\Microsoft-Windows-Windows Defender/WHC\" /v \"Enabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows Defender\" /v \"DisableAntiSpyware\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Microsoft\\Windows Defender\" /v \"DisableAntiVirus\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System\" /v \"EnableSmartScreen\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"AllowFastServiceStartup\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"DisableAntiSpyware\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\" /v \"ServiceKeepAlive\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen\" /v \"ConfigureAppInstallControlEnabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Notifications\" /v \"DisableNotifications\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows Defender Security Center\\Systray\" /v \"HideSystray\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\CI\\Config\" /v \"VulnerableDriverBlocklistEnable\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\CI\\Policy\" /v \"VerifiedAndReputablePolicyState\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\CredentialGuard\" /v \"Enabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\HypervisorEnforcedCodeIntegrity\" /v \"Enabled\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v \"MitigationAuditOptions\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v \"MitigationOptions\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\Sense\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmAgent\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmAgent\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"DelayedAutoStart\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\SgrmBroker\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdBoot\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdBoot\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdFilter\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdFilter\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisDrv\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisDrv\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WdNisSvc\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefsvc\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefsvc\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\webthreatdefusersvc\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\WinDefend\" /v \"Start\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"DelayedAutoStart\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"ErrorControl\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"LaunchProtected\" /f >NUL 2>nul",
            "reg delete \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\wscsvc\" /v \"Start\" /f >NUL 2>nul"
        };

                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }

            // Şimdi bat dosyasını çalıştır
            DefenderEnableCommand(batFilePath);

            // Bat dosyasını sildikten sonra temizle
            File.Delete(batFilePath);
        }

        private void DefenderEnableCommand(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process.StartInfo = startInfo;
            process.Start();

            // Çıktıyı oku
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            // Çıktıyı gerektiğine göre işle
            // Gerekiyorsa kaydet veya görüntüle
        }

        // OneDrive
        private void oneDriveRemove_Checked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "OneDrive.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
               "@echo off",
               "tasklist /fi \"ImageName eq OneDrive.exe\" /fo csv 2>NUL | find /i \"OneDrive.exe\">NUL && (",
               "    echo OneDrive.exe is running and will be killed.",
               "    taskkill /f /im OneDrive.exe",
               ") || (",
               "    echo Skipping, OneDrive.exe is not running.",
               ")",
               "if exist \"%SystemRoot%\\System32\\OneDriveSetup.exe\" (",
               "    \"%SystemRoot%\\System32\\OneDriveSetup.exe\" /uninstall",
               ") else (",
               "    if exist \"%SystemRoot%\\SysWOW64\\OneDriveSetup.exe\" (",
               "        \"%SystemRoot%\\SysWOW64\\OneDriveSetup.exe\" /uninstall",
               "    ) else (",
               "        echo Failed to uninstall, uninstaller could not be found. 1>&2",
               "    )",
               ")",
               "reg delete \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run\" /v \"OneDrive\" /f 2>nul",
               "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive\" /t REG_DWORD /v \"DisableFileSyncNGSC\" /d 1 /f",
               "reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive\" /t REG_DWORD /v \"DisableFileSync\" /d 1 /f",
               "reg add \"HKCR\\CLSID\\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\" /v \"System.IsPinnedToNameSpaceTree\" /d \"0\" /t REG_DWORD /f",
               "reg add \"HKCR\\Wow6432Node\\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\" /v \"System.IsPinnedToNameSpaceTree\" /d \"0\" /t REG_DWORD /f",
               "reg delete \"HKCU\\Environment\" /v \"OneDrive\" /f 2>nul",
               "PowerShell -ExecutionPolicy Unrestricted -Command \"$tasks=$(; Get-ScheduledTask 'OneDrive Reporting Task-*'; Get-ScheduledTask 'OneDrive Standalone Update Task-*'; ); if($tasks.Length -eq 0) {; Write-Host 'Skipping, no OneDrive tasks exists.'; } else {; Write-Host '^\"Total found OneDrive tasks: $($tasks.Length).^\"'; foreach ($task in $tasks) {; $fullPath = $task.TaskPath + $task.TaskName; Write-Host '^\"Deleting ^\"$fullPath^\"^\"'^\"; schtasks /DELETE /TN '^\"$fullPath^\"' /f; }; }\"",
               "PowerShell -ExecutionPolicy Unrestricted -Command \"reg delete '^\"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run^\"' /v '^\"OneDriveSetup^\"' /f 2>$null\"",
               "if exist \"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Microsoft OneDrive.lnk\" (",
               "    del \"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Microsoft OneDrive.lnk\" /s /f /q",
               ")",
               "if exist \"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" (",
               "    del \"%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" /s /f /q",
               ")",
               "if exist \"%USERPROFILE%\\Links\\OneDrive.lnk\" (",
               "    del \"%USERPROFILE%\\Links\\OneDrive.lnk\" /s /f /q",
               ")",
               "if exist \"%SystemDrive%\\Windows\\ServiceProfiles\\LocalService\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" (",
               "    del \"%SystemDrive%\\Windows\\ServiceProfiles\\LocalService\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" /s /f /q",
               ")",
               "if exist \"%SystemDrive%\\Windows\\ServiceProfiles\\NetworkService\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" (",
               "    del \"%SystemDrive%\\Windows\\ServiceProfiles\\NetworkService\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\OneDrive.lnk\" /s /f /q",
               ")",
               "PowerShell -ExecutionPolicy Unrestricted -Command \"Set-Location '^\"HKCU:\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Desktop\\NameSpace^\"'; Get-ChildItem | ForEach-Object {Get-ItemProperty $_.pspath} | ForEach-Object {; $leftnavNodeName = $_.'^\"(default)^\"';; if (($leftnavNodeName -eq '^\"OneDrive^\"') -Or ($leftnavNodeName -eq '^\"OneDrive - Personal^\"')) {; if (Test-Path $_.pspath) {; Write-Host '^\"Deleting $($_.pspath).^\"'; Remove-Item $_.pspath;; }; }; }\"",
               "if exist \"%UserProfile%\\OneDrive\" (",
               "    rd \"%UserProfile%\\OneDrive\" /q /s",
               ")",
               "if exist \"%LocalAppData%\\Microsoft\\OneDrive\" (",
               "    rd \"%LocalAppData%\\Microsoft\\OneDrive\" /q /s",
               ")",
               "if exist \"%ProgramData%\\Microsoft OneDrive\" (",
               "    rd \"%ProgramData%\\Microsoft OneDrive\" /q /s",
               ")",
               "if exist \"%SystemDrive%\\OneDriveTemp\" (",
               "    rd \"%SystemDrive%\\OneDriveTemp\" /q /s",
               ")"
            };


                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }

            // Şimdi bat dosyasını çalıştır
            OneDriveCommand(batFilePath);

            // Bat dosyasını sildikten sonra temizle
            File.Delete(batFilePath);
        }

        private void OneDriveCommand(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process.StartInfo = startInfo;
            process.Start();

            // Çıktıyı oku
            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            // Çıktıyı gerektiğine göre işle
            // Gerekiyorsa kaydet veya görüntüle
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

        // Explorer'daki "Ağ" klasörünü kaldırır
        private void explorerNetworkFolder_Checked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "explorerNetworkFolder.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
       "@echo off",
       "reg add \"HKEY_CLASSES_ROOT\\CLSID\\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\\ShellFolder\" /v \"Attributes\" /t REG_DWORD /d 0xb0940064 /f",
        "taskkill /f /im explorer.exe && start explorer.exe"
                };


                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }
            explorerNetworkFolderCommand(batFilePath);
            File.Delete(batFilePath);
        }

        private void explorerNetworkFolderCommand(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private void explorerNetworkFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "explorerNetworkFolder.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
       "@echo off",
       "reg add \"HKEY_CLASSES_ROOT\\CLSID\\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\\ShellFolder\" /v \"Attributes\" /t REG_DWORD /d 0xb0040064 /f",
       "taskkill /f /im explorer.exe && start explorer.exe"
        };


                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }
            explorerNetworkFolder2Command(batFilePath);
            File.Delete(batFilePath);
        }

        private void explorerNetworkFolder2Command(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
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
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", 0, RegistryValueKind.DWord);
        }

        private void teams_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarMn", 1, RegistryValueKind.DWord);
        }

        // Remove Microsoft Teams Icon on Taskbar
        private void bing_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCloudSearch", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "ConnectedSearchUseWeb", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "DisableWebSearch", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
        }

        private void bing_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCloudSearch", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Policies\Microsoft\Windows\Windows Search", "DisableWebSearch", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "ConnectedSearchUseWeb", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "DisableWebSearch", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 0, RegistryValueKind.DWord);
        }

        // Remove Microsoft Teams Icon on Taskbar
        private void disableUAC_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorUser", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableInstallerDetection", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableSecureUIAPaths", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 0, RegistryValueKind.DWord);
        }

        private void disableUAC_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", 5, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorUser", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableInstallerDetection", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableSecureUIAPaths", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "PromptOnSecureDesktop", 1, RegistryValueKind.DWord);
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
                    File.Delete(filePathToDelete);                }
                else
                {
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

        // Disable Windows Search
        private void disableWindowsSearch_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\WSearch", "Start", 4, RegistryValueKind.DWord);

        }

        private void disableWindowsSearch_Unchecked(object sender, RoutedEventArgs e)
        {
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

        private void BitLockerDriveEncryptionService_Checked(object sender, RoutedEventArgs e)
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
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wisvc", "Start", 4, RegistryValueKind.DWord);
        }

        private void WindowsInsiderService_Unchecked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\wisvc", "Start", 3, RegistryValueKind.DWord);
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
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\wscsvc", true))
                {
                    if (key != null)
                    {
                        key.SetValue("Start", 4, RegistryValueKind.DWord);
                    }
                    else
                    {
                        // Anahtar bulunamadı
                    }
                }
            }
            catch (Exception)
            {
                // Hata işleme
            }
        }

        private void SecurityCenterService_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\wscsvc", true))
                {
                    if (key != null)
                    {
                        key.SetValue("Start", 2, RegistryValueKind.DWord);
                    }
                    else
                    {
                        // Anahtar bulunamadı
                    }
                }
            }
            catch (Exception)
            {
                // Hata işleme
            }
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

        // Clock Second

        private void clockSecond_Checked(object sender, RoutedEventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 1, RegistryValueKind.DWord);
        }


        private void clockSecond_Unchecked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 0, RegistryValueKind.DWord);
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
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "oldPhotoViewer.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
                "@echo off",
                "reg add \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\" /v \"MuiVerb\" /t REG_EXPAND_SZ /d \"@photoviewer.dll,-3043\"",
                "reg add \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\\command\" /ve /t REG_EXPAND_SZ /d \"\\\"%SystemRoot%\\System32\\rundll32.exe\\\" \\\"%ProgramFiles%\\Windows Photo Viewer\\PhotoViewer.dll\\\", ImageView_Fullscreen %1\"",
                "reg add \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\\DropTarget\" /v \"Clsid\" /t REG_SZ /d \"{FFE2A43C-56B9-4bf5-9A79-CC6D4285608A}\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".bmp\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".dib\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".gif\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".ico\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jfif\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpe\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpeg\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpg\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jxr\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".png\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".tif\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\"",
                "reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".tiff\" /t REG_SZ /d \"PhotoViewer.FileAssoc.Tiff\""
            };



                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }

            // Şimdi bat dosyasını çalıştır
            oldPhotoViewerEnable(batFilePath);

            // Bat dosyasını sildikten sonra temizle
            File.Delete(batFilePath);
        }

        private void oldPhotoViewerEnable(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        private void oldPhotoViewer_Unchecked(object sender, RoutedEventArgs e)
        {
            string batFilePath = Path.Combine(Environment.CurrentDirectory, "oldPhotoViewer.bat");

            using (StreamWriter writer = new StreamWriter(batFilePath))
            {
                string[] batchCommands = {
                "@echo off",
                "reg delete \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\" /f",
                "reg delete \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\\command\" /f",
                "reg delete \"HKEY_CLASSES_ROOT\\Applications\\photoviewer.dll\\shell\\open\\DropTarget\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".bmp\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".dib\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".gif\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".ico\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jfif\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpe\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpeg\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jpg\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".jxr\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".png\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".tif\" /f",
                "reg delete \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Photo Viewer\\Capabilities\\FileAssociations\" /v \".tiff\" /f"
            };



                foreach (string command in batchCommands)
                {
                    writer.WriteLine(command);
                }
            }

            // Şimdi bat dosyasını çalıştır
            oldPhotoViewerDisable(batFilePath);

            // Bat dosyasını sildikten sonra temizle
            File.Delete(batFilePath);
        }

        private void oldPhotoViewerDisable(string batFilePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "MinSudo.exe",
                Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose cmd.exe /C \"{batFilePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Environment.CurrentDirectory
            };

            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }

        // İnternet'ten indirilen dosyaları başlatırken güvenlik uyarılarını devre dışı bırak

        private void attachmentManager_Checked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 1, RegistryValueKind.DWord);
        }

        private void attachmentManager_Unchecked(object sender, EventArgs e) 
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Policies\Attachments", "SaveZoneInformation", 0, RegistryValueKind.DWord);
        }

        // Explorer.exe Optimizasyonu

        private void explorerOptimization_Checked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SeparateProcess", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications", 0, RegistryValueKind.DWord);
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

        private void explorerOptimization_Unchecked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 1, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SeparateProcess", 0, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSyncProviderNotifications", 0, RegistryValueKind.DWord);
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

        // Başlangıçda NumLock etkinleştir
        private void NumLock_Checked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators", "2", RegistryValueKind.String);
        }
        private void NumLock_Unchecked(object sender, EventArgs e)
        {
            RegHelper.SetValue(RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators", "0", RegistryValueKind.String);
        }

        // Cleaning Kodları başlangıcı!

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            long totalSize = 0;
            // Analiz işlemi başladığında ProgressBar'ı görünür yap
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true; // Dönme başlasın

            sizeLabel.Content = "";
            outputTextBox.Text = "";

            List<CheckBox> selectedCheckBoxes = GetSelectedCheckBoxes();

            if (selectedCheckBoxes.Count == 0)
            {
                System.Windows.MessageBox.Show("Lütfen en az bir seçenek işaretleyin.");
                // Analiz işlemi tamamlandığında ProgressBar'ı gizle
                progressBar.Visibility = Visibility.Hidden;
                progressBar.IsIndeterminate = false; // Dönme durdu
                return;
            }

            List<string> folderPaths = new List<string>();

            foreach (var checkBox in selectedCheckBoxes)
            {
                string folderPath = GetFolderPathForCheckbox(checkBox.Name);

                if (!string.IsNullOrEmpty(folderPath))
                {
                    var directoryInfo = new DirectoryInfo(folderPath);
                    totalSize += await AnalyzeDirectoryAsync(directoryInfo, folderPaths);
                }
            }

            sizeLabel.Content = $"{FindResource("sizeLabelContent")} {FormatFileSize(totalSize)}";
            outputTextBox.Text = string.Join(Environment.NewLine, folderPaths);

            // Analiz işlemi tamamlandığında ProgressBar'ı gizle
            progressBar.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false; // Dönme durdu
        }



        private async void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            // Temizleme işlemi başladığında ProgressBar'ı görünür yap
            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true; // Dönme başlasın

            List<CheckBox> selectedCheckBoxes = GetSelectedCheckBoxes();

            if (selectedCheckBoxes.Count == 0)
            {
                System.Windows.MessageBox.Show("Lütfen en az bir seçenek işaretleyin.");
                // Temizleme işlemi tamamlandığında ProgressBar'ı gizle
                progressBar.Visibility = Visibility.Hidden;
                return;
            }

            foreach (var checkBox in selectedCheckBoxes)
            {
                string folderPath = GetFolderPathForCheckbox(checkBox.Name);

                if (!string.IsNullOrEmpty(folderPath))
                {
                    await CleanDirectoryAsync(folderPath);
                }
            }

            progressBar.Visibility = Visibility.Hidden;
            progressBar.IsIndeterminate = false; // Dönme durdu
            System.Windows.MessageBox.Show("Cleanup successfully completed!");
        }

        private async Task<long> AnalyzeDirectoryAsync(DirectoryInfo directoryInfo, List<string> filePaths)
        {
            long totalSize = 0;

            if (directoryInfo.Exists)
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    totalSize += file.Length;
                    filePaths.Add(file.FullName); // Dosya yollarını ekleyin
                }

                foreach (var subDirectory in directoryInfo.GetDirectories())
                {
                    string powershellCommand = $"(Get-ChildItem -Recurse '{subDirectory.FullName}' | Measure-Object -Property Length -Sum).Sum";
                    long subDirectorySize = await RunPowerShellCommandAsync(powershellCommand);
                    totalSize += subDirectorySize;

                    // İlerlemeyi güncelle
                    ReportProgress(totalSize, totalSize);

                    // Alt klasörleri yeniden analiz edin ve dosya yollarını ekleyin
                    long subDirectoryFilesSize = await AnalyzeDirectoryAsync(subDirectory, filePaths);
                    totalSize += subDirectoryFilesSize;
                }
            }

            return totalSize; // totalSize değerini geri döndür
        }



        private async Task CleanDirectoryAsync(string directoryPath)
        {
            try
            {
                // Klasör içeriğini silmek için komut
                string deleteContentCommand = $"cmd.exe /C del /Q /F /S \"{directoryPath}\\*.*\"";
                Process deleteContentProcess = new Process();

                ProcessStartInfo deleteContentStartInfo = new ProcessStartInfo
                {
                    FileName = "MinSudo.exe",
                    Arguments = $"--TrustedInstaller --NoLogo --Privileged --Verbose {deleteContentCommand}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.CurrentDirectory
                };

                deleteContentProcess.StartInfo = deleteContentStartInfo;
                deleteContentProcess.Start();

                string deleteContentOutput = await deleteContentProcess.StandardOutput.ReadToEndAsync();
                deleteContentProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }


        private List<CheckBox> GetSelectedCheckBoxes()
        {
            List<CheckBox> checkBoxes = new List<CheckBox>
            {
                logsCheckbox, tempCheckbox, windowsOldCheckbox,
                errorReportingCheckbox, liveKernelCachesCheckbox,
                downloadsCheckbox, recycleBinCheckbox, searchIndexCheckbox,
                prefetchCheckbox, fontCacheCheckbox, installerCheckbox, softwareDistributionCheckbox,
                googleChromeCacheCheckbox, otherLogsCheckbox, edgeCacheCheckbox
            };

            return checkBoxes.Where(cb => cb.IsChecked == true).ToList();
        }

        private string GetFolderPathForCheckbox(string checkboxName)
        {
            switch (checkboxName)
            {
                // Checkbox'lara uygun klasör yollarını ekleyin.
                case "logsCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\logs";
                case "otherLogsCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\System32\LogFiles";
                case "tempCheckbox":
                    return Path.GetTempPath();
                case "windowsOldCheckbox":
                    return @"C:\Windows.old";
                case "errorReportingCheckbox":
                    return @"C:\ProgramData\Microsoft\Windows\WER";
                case "liveKernelCachesCheckbox":
                    return @"C:\Windows\LiveKernelReports";
                case "downloadsCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
                case "recycleBinCheckbox":
                    string recycleBinPath = @"C:\$Recycle.Bin";
                    return recycleBinPath;
                case "searchIndexCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Microsoft\Search\Data\Applications\Windows";
                case "prefetchCheckbox":
                    return @"C:\\Windows\\Prefetch";
                case "fontCacheCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\ServiceProfiles\LocalService\AppData\Local\FontCache";
                case "installerCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Installer";
                case "softwareDistributionCheckbox":
                    return @"C:\Windows\SoftwareDistribution";
                case "googleChromeCacheCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cache\";
                case "edgeCacheCheckbox":
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data\Default\Cache\";
                // Diğer checkbox'lar için aynı şekilde klasör yollarını ekleyin.
                default:
                    return null;
            }
        }

        private async Task<long> RunPowerShellCommandAsync(string command)
        {
            long result = 0;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "powershell";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                if (process.ExitCode == 0 && long.TryParse(output, out result))
                {
                    return result;
                }
            }

            return result;
        }

        private void ReportProgress(long current, long total)
        {
            if (total == 0)
            {
                // total sıfır ise, progressPercentage'yi 0 olarak ayarlayın
                Dispatcher.Invoke(() =>
                {
                    progressBar.Value = 0;
                });
            }
            else
            {
                double progressPercentage = (double)current / total * 100;
                Dispatcher.Invoke(() =>
                {
                    progressBar.Value = progressPercentage;
                });
            }
        }


        private string FormatFileSize(long bytes)
        {
            string[] sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            const int unit = 1024;

            if (bytes == 0)
                return "0 bytes";

            int order = (int)Math.Log(bytes, unit);
            double adjustedSize = bytes / Math.Pow(unit, order);
            string sizeSuffix = sizeSuffixes[order];

            return $"{adjustedSize:0.##} {sizeSuffix}";
        }

        private void CheckForUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the URL of the version file
            string versionUrl = "https://github.com/shadesofdeath/ShadesTweaker/raw/main/version.txt";

            try
            {
                // Delete the previously downloaded file if it exists
                if (File.Exists("version.txt"))
                {
                    File.Delete("version.txt");
                }

                // Download the version number from the URL
                WebClient webClient = new WebClient();
                webClient.DownloadFile(versionUrl, "version.txt");

                // Read the downloaded version number
                string latestVersion = File.ReadAllText("version.txt");

                // Get the current version number of your application (you may store it as a variable in your application)
                string currentVersion = "1.6"; // Update this with your application's actual version number

                // Compare the version numbers
                if (latestVersion.Trim() == currentVersion.Trim())
                {
                    System.Windows.MessageBox.Show("No updates found.");
                }
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("An update is available. Download the new version.", "Update Found", MessageBoxButton.OKCancel);

                    if (result == MessageBoxResult.OK)
                    {
                        // Redirect the user to the specified website when they click "OK"
                        System.Diagnostics.Process.Start("https://github.com/shadesofdeath/ShadesTweaker/releases");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred during the update check: " + ex.Message);
            }
        }
    }
}