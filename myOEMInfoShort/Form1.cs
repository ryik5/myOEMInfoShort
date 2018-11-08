using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices; //SetUserDesktop

namespace OEMInfo
{
    public partial class Form1 : Form
    {
        private Bitmap LogoWindows;
        private Bitmap OEMWalpaper;
        private string Model;
        private string Manufacturer;
        private string WindowsPath;
        private string TypeSystem;
        private List<string> lOEM = new List<string>();
        private List<string> lUSB = new List<string>();
        private List<string> lSoftware = new List<string>();

        public Form1()
        { InitializeComponent(); }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Diagnostics.FileVersionInfo myFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            LogoWindows = new Bitmap(Properties.Resources.logo);
            Icon = Properties.Resources.IconRYIK;

            // Autorun allFunction
            GetInfo();
            SetDesktopInfo();
            FormClosedFull();
        }

        private static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !ip.ToString().Contains("127.0.0.1"))
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

         private void GetInfo()
        {
            lOEM.Clear();
            lSoftware.Clear();
            lUSB.Clear();

            string ia;

            try
            {
                ManagementObjectCollection moReturn;
                ManagementObjectSearcher moSearch;

                moSearch = new ManagementObjectSearcher("Select * from Win32_OperatingSystem");
                moReturn = moSearch.Get();
                foreach (ManagementObject mo in moReturn)
                {
                    try
                    {
                        if (mo["ProductType"] != null) ia = CheckTypeOS(Convert.ToInt32(mo["ProductType"].ToString())); else ia = "";
                        TypeSystem = ia;
                    }
                    catch { }
                }

                //NET
                lOEM.Add("-= NET =-");
                lOEM.Add(TypeSystem + ": " + System.Net.Dns.GetHostName());
                lOEM.Add("IP: " + GetLocalIPAddress());
                lOEM.Add("");

                //CPU
                    lOEM.Add("-= CPU =-");
                moSearch = new ManagementObjectSearcher("Select Name,ProcessorID,SocketDesignation from Win32_Processor");
                moReturn = moSearch.Get();
                foreach (ManagementObject mo in moReturn)
                {
                    ia = mo["Name"].ToString().Trim().ToUpper();
                    //     Model += ia + " ";
                    lOEM.Add("Name: " + ia);
                    try
                    {
                        ia = mo["ProcessorID"].ToString().Trim().ToUpper();
                        lOEM.Add("ProcessorID: " + ia);
                        ia = mo["SocketDesignation"].ToString().Trim().ToUpper();
                        if (!ia.ToUpper().Contains("N/A")) lOEM.Add("SocketDesignation: " + ia);
                    }
                    catch { }
                    lOEM.Add("");
                }

                //Motherboard
                    lOEM.Add("-= Motherboard =-");
                moSearch = new ManagementObjectSearcher("Select * from Win32_BaseBoard");//Manufacturer,Model,Product,Version,SerialNumber,OtherIdentifyingInfo,PartNumber
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    ia = mo["Manufacturer"].ToString().Trim().ToUpper();
                    Manufacturer += ia + " ";
                    lOEM.Add("Manufacturer: " + ia);
                    try
                    {
                        ia = mo["Model"].ToString().Trim().ToUpper();
                        lOEM.Add("Model: " + ia);
                    }
                    catch { }
                    try
                    {
                        ia = mo["OtherIdentifyingInfo"].ToString().Trim().ToUpper();
                        lOEM.Add("OtherIdentifyingInfo: " + ia);
                    }
                    catch { }
                    ia = mo["Product"].ToString().Trim().ToUpper();
                    Model += "MB: " + ia + " ";
                    lOEM.Add("Product: " + ia);
                    try
                    {
                        ia = mo["PartNumber"].ToString().Trim().ToUpper();
                        lOEM.Add("PartNumber: " + ia);
                    }
                    catch { }
                    ia = mo["Version"].ToString().Trim().ToUpper();
                    if (!ia.ToUpper().Contains("N/A")) lOEM.Add("Version: " + ia);
                    ia = mo["SerialNumber"].ToString().Trim().ToUpper();
                    if (!ia.ToUpper().Contains("SERIAL NUMBER") && !ia.ToUpper().Contains("N/A") && !ia.ToUpper().Contains("O.E.M.") && !ia.ToUpper().Contains("EMPTY") && ia.Trim().Length > 1) lOEM.Add("Serial Number: " + ia);
                    lOEM.Add("");
                }

                //BIOS
                    lOEM.Add("-= BIOS =-");
                moSearch = new ManagementObjectSearcher("Select SerialNumber,Manufacturer,SMBIOSBIOSVersion,Version from Win32_BIOS");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    ia = mo["Manufacturer"].ToString().Trim().ToUpper();
                    lOEM.Add("Manufacturer: " + ia);
                    ia = mo["SMBIOSBIOSVersion"].ToString().Trim().ToUpper();
                    lOEM.Add("SMBIOSBIOSVersion: " + ia);
                    ia = mo["Version"].ToString().Trim().ToUpper();
                    lOEM.Add("Version: " + ia);
                    ia = mo["SerialNumber"].ToString().Trim().ToUpper();
                    if (!ia.ToUpper().Contains("SERIAL NUMBER") && !ia.ToUpper().Contains("O.E.M.") && !ia.ToUpper().Contains("EMPTY") && ia.Trim().Length > 1) lOEM.Add("Serial Number: " + ia);
                    lOEM.Add("");
                }

                //Video
                    lOEM.Add("-= Video =-");
                moSearch = new ManagementObjectSearcher("Select Caption,AdapterRAM,DriverVersion,AdapterDACType from Win32_VideoController");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    try
                    {
                        ia = mo["Caption"].ToString().Trim().ToUpper();
                        Model += @"     Video: " + ia + " ";
                        lOEM.Add("Caption: " + ia);
                    }
                    catch { }
                    try
                    {
                        double ib = Convert.ToDouble(mo["AdapterRAM"].ToString().Trim()) / 1024 / 1024;
                        lOEM.Add("Adapter RAM, MB: " + ib.ToString("#.##") + " MB");
                        ia = mo["DriverVersion"].ToString().Trim().ToUpper();
                        lOEM.Add("Driver Version: " + ia);
                        ia = mo["AdapterDACType"].ToString().Trim().ToUpper();
                        lOEM.Add("AdapterDACType: " + ia);
                    }
                    catch { }
                    lOEM.Add("");
                }

                //RAM
                    lOEM.Add("-= RAM =-");
                ulong installedMemory;
                MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
                    if (GlobalMemoryStatusEx(memStatus))
                {
                    installedMemory = memStatus.ullTotalPhys;
                    lOEM.Add("Available Memory: " + Math.Round((double)installedMemory / 1024 / 1024, 0) + " MB");
                }

                moSearch = new ManagementObjectSearcher("Select * from Win32_PhysicalMemory");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    string sMod = "";
                    try
                    {
                        sMod = ",  Model - " + mo["Model"].ToString().ToUpper().Trim();
                    }
                    catch { }
                    try
                    {
                        if (!mo["SerialNumber"].ToString().ToUpper().Contains("SERNUM") &&
                             mo["SerialNumber"].ToString().Trim().Length < 2)
                            sMod += ",  s/n - " + mo["SerialNumber"].ToString().ToUpper().Trim();
                    }
                    catch { }
                    try
                    {
                        double dSpeed = Convert.ToDouble(mo["Speed"].ToString().Trim());
                        if (dSpeed > 10)
                            sMod += ",  Type - " + dSpeed + " ";
                        else
                            sMod += ",  Freq - " + Math.Round((1000 / dSpeed), 0) + " MHz";
                    }
                    catch { }

                    lOEM.Add("banklabel: " + mo["banklabel"].ToString().ToUpper().Trim()
                        + "  " + (Convert.ToDouble(mo["Capacity"].ToString().Trim()) / 1024 / 1024).ToString("#") + " MB,  Type - " +
                        CheckTypeMemory(Convert.ToInt32(mo["MemoryType"].ToString().Trim())) + ",  Formfactor - " +
                        CheckFormFactorMemory(Convert.ToInt32(mo["FormFactor"].ToString().Trim())));
                    if (sMod.Trim().Length > 1) lOEM.Add("     " + sMod);
                }

                //RAM Win32_PhysicalMemoryArray
                moSearch = new ManagementObjectSearcher("Select * from Win32_PhysicalMemoryArray");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    if (mo["MaxCapacity"].ToString().Trim().Length < 0) lOEM.Add("Maximum size: " + (Convert.ToDouble(mo["MaxCapacity"].ToString().Trim()) / 1024 / 1024).ToString("#") + " GB");
                }
                    lOEM.Add("");


                //Mass Storage
                    lOEM.Add("-= Mass Storage =-");
                moSearch = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    try
                    {
                        ia = mo["Caption"].ToString().ToUpper();
                        lOEM.Add("Caption: " + ia);
                    }
                    catch { }
                    try
                    {
                        double ib = Convert.ToDouble(mo["Size"].ToString().Trim()) / 1024 / 1024 / 1024;
                        lOEM.Add("Size: " + ib.ToString("#.##") + " GB,  " + "InterfaceType: " + mo["InterfaceType"].ToString().ToUpper().Trim());
                    }
                    catch { }
                    try
                    {
                        ia = mo["Signature"].ToString().ToUpper().Trim();
                        //                lOEM.Add("Signature: " + ia);
                        ia = mo["Partitions"].ToString().ToUpper().Trim();
                        // lOEM.Add("Partitions: " + ia);
                    }
                    catch { }
                    try
                    {
                        ia = mo["SerialNumber"].ToString().Trim().ToUpper();
                        lOEM.Add("SerialNumber: " + ia);
                    }
                    catch { }
                    try
                    {
                        //   ia = mo["MediaType"].ToString().ToUpper();
                        //   lOEM.Add("MediaType: " + ia);
                    }
                    catch { }
                    lOEM.Add("");
                }

                //OS
                    lOEM.Add("-= OS =-");
                moSearch = new ManagementObjectSearcher("Select * from Win32_OperatingSystem");
                moReturn = moSearch.Get();
                    foreach (ManagementObject mo in moReturn)
                {
                    if (mo["Caption"] != null) ia = mo["Caption"].ToString().Trim().ToUpper(); else ia = "";
                    lOEM.Add("Name: " + ia);

                    if (mo["OperatingSystemSKU"] != null) ia = CheckTypeProductOS(Convert.ToInt32(mo["OperatingSystemSKU"].ToString().Trim())); else ia = "";
                    lOEM.Add("Product Info: " + ia);
                    if (mo["CSDVersion"] != null) ia = mo["CSDVersion"].ToString().Trim().ToUpper(); else ia = "";
                    if (mo["Version"] != null) ia += "." + mo["Version"].ToString().Trim().ToUpper(); else ia += "";
                    if (mo["BuildNumber"] != null) ia += "." + mo["BuildNumber"].ToString().Trim().ToUpper(); else ia += "";

                    lOEM.Add("SP: " + ia);
                    if (mo["OSArchitecture"] != null) ia = mo["OSArchitecture"].ToString().Trim().ToUpper(); else ia = "";
                    lOEM.Add("OS Arch: " + ia);

                    if (mo["ProductType"] != null) ia = CheckTypeOS(Convert.ToInt32(mo["ProductType"].ToString().Trim())); else ia = "";
                    lOEM.Add("Type of System: " + ia);
                    TypeSystem = ia;

                    if (mo["SystemDirectory"] != null) ia = mo["SystemDirectory"].ToString().Trim().ToUpper(); else ia = "";
                    lOEM.Add("System directory of OS: " + ia);
                    WindowsPath = ia;

                    var key = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
                    key = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
                    if (key != null)
                    {
                        DateTime startDate = new DateTime(1970, 1, 1, 0, 0, 0);

                        object objValue = key.GetValue("InstallDate"); //InstallTime
                        string stringValue = objValue.ToString();
                        Int64 regVal = Convert.ToInt64(stringValue);

                        DateTime installDate = startDate.AddSeconds(regVal);
                        ia = installDate.ToString("yyyy-MM-dd HH:MM");
                    }

                    lOEM.Add("Install Date: " + ia);
                    if (mo["LastBootUpTime"] != null) ia = mo["LastBootUpTime"].ToString(); else ia = "";
                    DateTime LastBootUpTime = ManagementDateTimeConverter.ToDateTime(ia);
                    lOEM.Add("Last BootUp Time: " + LastBootUpTime.ToString("yyyy-MM-dd HH:MM"));
                    double dDay = 0; string sUpTime = "";
                    if (UpTime.TotalHours / 24 > 1) dDay = Math.Round(UpTime.TotalHours / 24, 0);
                    if (dDay > 0)
                        sUpTime = dDay.ToString() + " days and " + (UpTime.TotalHours - dDay * 24).ToString("#.##" + " hours");
                    else
                        sUpTime = UpTime.TotalHours.ToString("#.##" + " hours");
                    lOEM.Add("Up Time: " + sUpTime);

                }
                moSearch.Dispose();
                moReturn.Dispose();
            }
            catch (Exception expt) { MessageBox.Show(expt.ToString()); }

             GetOtherSoftwareInfo(); 
             SearchUSB();  
        }

        private void GetOtherSoftwareInfo()   //Intellect     //Autocod        
        {
            string sSoftwareVersion = "";
            string sInstallPath = "";
            string sAddInfo = "";
            string sIntellectRegPath = @"SOFTWARE\Wow6432Node\ITV\INTELLECT";   //Check Intellect

            sIntellectRegPath = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Intellect";   //Check Intellect
            using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sIntellectRegPath, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
            {
                try
                {
                    sAddInfo = EvUserKey.GetValue("DisplayName").ToString().Trim();
                    sInstallPath = EvUserKey.GetValue("InstallLocation").ToString().Trim();
                    sSoftwareVersion = EvUserKey.GetValue("DisplayVersion").ToString().Trim();
                    lSoftware.Add("-= " + sAddInfo + " =-");
                    lSoftware.Add("Path: " + sInstallPath);
                    lSoftware.Add("Version:" + sSoftwareVersion);
                    lSoftware.Add("");
                }
                catch { }
            }

            sIntellectRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\AutoCode Client_is1"; //Check Autocod
            using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sIntellectRegPath, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
            {
                try
                {
                    sAddInfo = EvUserKey.GetValue("DisplayName").ToString().Trim();
                    sInstallPath = EvUserKey.GetValue("InstallLocation").ToString().Trim();
                    sSoftwareVersion = EvUserKey.GetValue("DisplayVersion").ToString().Trim();
                    lSoftware.Add("-= " + sAddInfo + " =-");
                    lSoftware.Add("Path: " + sInstallPath);
                    lSoftware.Add("Version:" + sSoftwareVersion);
                    lSoftware.Add("");
                }
                catch { }
            }

            sIntellectRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\AutoCode Server_is1"; //Check Autocod
            using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sIntellectRegPath, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
            {
                try
                {
                    sAddInfo = EvUserKey.GetValue("DisplayName").ToString().Trim();
                    sInstallPath = EvUserKey.GetValue("InstallLocation").ToString().Trim();
                    sSoftwareVersion = EvUserKey.GetValue("DisplayVersion").ToString().Trim();
                    lSoftware.Add("-= " + sAddInfo + " =-");
                    lSoftware.Add("Path: " + sInstallPath);
                    lSoftware.Add("Version:" + sSoftwareVersion);
                    lSoftware.Add("");
                }
                catch { }
            }
        }

        private void SearchUSB()
        {
            lUSB.Clear();
            string _USBKeyString = "SYSTEM\\CurrentControlSet\\Enum\\USBSTOR";
            try
            {
                using (var usbBaseKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString, false))
                {
                    string[] subNames = usbBaseKey.GetSubKeyNames();
                    lUSB.Add("-= Used USB Drives=-");
                    foreach (string s in subNames)                                                         //Get key of USB
                    {
                        if (s != null)
                        {
                            string _USBKeyString1 = _USBKeyString + "\\" + s;
                            using (var usbBaseKey1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString1, false))
                            {
                                foreach (string s1 in usbBaseKey1.GetSubKeyNames())                         //Get s/n of USB
                                {
                                    if (s1 != null)
                                    {
                                        string _USBKeyString2 = _USBKeyString1 + "\\" + s1;
                                        using (var usbBaseKey2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString2, false))
                                        {
                                            foreach (string valueName in usbBaseKey2.GetValueNames())     //Get Name of USB
                                            {
                                                if (valueName != null)
                                                {
                                                    string s2 = usbBaseKey2.GetValue(valueName).ToString();
                                                    if (valueName.Contains("FriendlyName") || valueName.Contains("ParentIdPrefix"))
                                                        try
                                                        {
                                                            bool sFullList = (s.ToUpper().Contains("USB") || s.ToUpper().Contains("DISK"));

                                                            if (sFullList && !(s1.Remove(s1.LastIndexOf('&'))).Contains('&')) lUSB.Add(s2 + " | с/н: " + s1.Remove(s1.LastIndexOf('&')));
                                                        }
                                                        catch { }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            try
            {
                //                    lUSB.Add("");
                _USBKeyString = "SYSTEM\\CurrentControlSet\\Enum\\USB";
                using (var usbBaseKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString, false))
                {
                    string[] subNames = usbBaseKey.GetSubKeyNames();

                    if (lUSB.ToArray().Length < 2) lUSB.Clear(); else lUSB.Add("");

                    lUSB.Add("-= The Full List Early Connected USB sticks =-");
                    foreach (string s in subNames)                                                         //Get key of USB
                    {
                        if (s != null)
                        {
                            string _USBKeyString1 = _USBKeyString + "\\" + s;
                            using (var usbBaseKey1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString1, false))
                            {
                                foreach (string s1 in usbBaseKey1.GetSubKeyNames())                         //Get s/n of USB
                                {
                                    if (s1 != null)
                                    {
                                        string _USBKeyString2 = _USBKeyString1 + "\\" + s1;
                                        using (var usbBaseKey2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_USBKeyString2, false))
                                        {
                                            foreach (string valueName in usbBaseKey2.GetValueNames())     //Get Name of USB
                                            {
                                                if (valueName != null)
                                                {
                                                    string s2 = usbBaseKey2.GetValue(valueName).ToString();
                                                    bool sFullList = valueName.Contains("DeviceDesc")
                                                        && s.ToUpper().Contains("VID")
                                                        && !s2.ToLower().Contains("broadband")
                                                        && !s2.ToLower().Contains("connect")
                                                        && !s2.ToLower().Contains("controller")
                                                        && !s2.ToLower().Contains("composite")
                                                        && !s2.ToLower().Contains("diagnostics")
                                                        && !s2.ToLower().Contains("hid-совместимое устройство")
                                                        && !s2.ToLower().Contains("generic usb hub")
                                                        && !s2.ToLower().Contains("input device")
                                                        && !s2.ToLower().Contains("keyboard")
                                                        && !s2.ToLower().Contains("interface")
                                                        && !s2.ToLower().Contains("mouse")
                                                        && !s2.ToLower().Contains("modem")
                                                        && !s2.ToLower().Contains("root hub")
                                                        && !s2.ToLower().Contains("webcam")
                                                        && !s2.ToLower().Contains("устройство ввода")
                                                        && !s2.ToLower().Contains("составное");
                                                    
                                                    if (sFullList)
                                                    {
                                                        lUSB.Add(s2.Substring(s2.IndexOf(';') + 1) + " | с/н: " + s1);
                                                        lUSB.Add("            ID: " + s.ToUpper().Replace('&', ' '));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private TimeSpan UpTime //Take the OS UP time
        {
            get
            {
                using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();       //Call this an extra time before reading its value
                    return TimeSpan.FromSeconds(uptime.NextValue());
                }
            }
        }
        
        private void SetDesktopInfo()  // Draw registration
        {
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            //   MessageBox.Show("Resolution: " + screenWidth + "x" + screenHeight);
            int iGapHorizontal = Screen.PrimaryScreen.Bounds.Width - 400;
            string pathDesktopBackground = GetWallpaper();
            //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            OEMWalpaper = new Bitmap(Properties.Resources.ColorBlack, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            var font = new Font("ARIAL", 8, FontStyle.Regular);
            if (Screen.PrimaryScreen.Bounds.Width - 400 < 1)
                iGapHorizontal = 10;

            using (Graphics gr = Graphics.FromImage(OEMWalpaper))
            {
                var myBrushFont = new SolidBrush(Color.PaleGreen);
                int iStringN = 0;
                foreach (string sTemp in lOEM.ToArray())
                {
                    iStringN += 12;
                    gr.DrawString(sTemp, font, myBrushFont, new Point(iGapHorizontal, iStringN));
                }

                myBrushFont = new SolidBrush(Color.DarkCyan);
                iStringN += 36;
                gr.DrawString("Дата проверки: " + DateTime.Now.ToShortDateString(), font, myBrushFont, new Point(iGapHorizontal, iStringN));
                iStringN += 12;
                gr.DrawString("OEMINFO  @RYIK 2017", font, myBrushFont, new Point(iGapHorizontal, iStringN));

                myBrushFont = new SolidBrush(Color.PaleGreen);
                iGapHorizontal = Screen.PrimaryScreen.Bounds.Width - 800;
                if (Screen.PrimaryScreen.Bounds.Width - 800 < 1 && Screen.PrimaryScreen.Bounds.Width - 400 < 200)
                { iGapHorizontal = 10; }
                else
                {
                    iGapHorizontal = Screen.PrimaryScreen.Bounds.Width - 800;
                    iStringN = 0;
                }

                foreach (string sTemp in lSoftware.ToArray())
                {
                    iStringN += 12;
                    gr.DrawString(sTemp, font, myBrushFont, new Point(iGapHorizontal, iStringN));
                }
                iStringN += 12;
                foreach (string sTemp in lUSB.ToArray())
                {
                    iStringN += 12;
                    gr.DrawString(sTemp, font, myBrushFont, new Point(iGapHorizontal, iStringN));
                }
                myBrushFont?.Dispose();
                font?.Dispose();
            }

            OEMWalpaper.Save(pathDesktopBackground, System.Drawing.Imaging.ImageFormat.Bmp);
            SetWallpaper(pathDesktopBackground, Style.Stretched);
            OEMWalpaper?.Dispose();
        }

        // set the desktop wallpaper
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);
        //private static UInt32 SPI_SETDESKWALLPAPER = 20; //slideshow
        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        public enum Style : int
        {
            Fill,
            Fit,
            Stretched
        }
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        private void SetWallpaper(String path, Style style)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
               0,
               path,
               SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
        }

        //Get the current path of the desktop wallpaper
        private static readonly UInt32 SPI_GETDESKWALLPAPER = 0x73;
        private static readonly int MAX_PATH = 260;
        private String GetWallpaper()
        {
            String wallpaperPath = new String('\0', MAX_PATH);

            try
            {
                SystemParametersInfo(SPI_GETDESKWALLPAPER, (UInt32)wallpaperPath.Length, wallpaperPath, 0);
                wallpaperPath = wallpaperPath.Substring(0, wallpaperPath.IndexOf('\0'));
            }
            catch
            {
                wallpaperPath = WindowsPath;
            }
            if (wallpaperPath.Length < 4 || !wallpaperPath.ToLower().Contains(".mpb"))
                wallpaperPath = System.IO.Path.GetTempPath() + "\\ServerOEM.bmp";

            return wallpaperPath;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        //Transform gotten data OEM INFO from system to understand info
        private string CheckTypeProductOS(int iNumber) //Type Product
        {
            string sTypeOs = "";
            switch (iNumber)
            {
                case 0:
                    sTypeOs = iNumber.ToString();
                    break;
                case 1:
                    sTypeOs = "Ultimate Edition ";
                    break;
                case 2:
                    sTypeOs = "Home Basic Edition";
                    break;
                case 3:
                    sTypeOs = "Home Premium Edition";
                    break;
                case 4:
                    sTypeOs = "Enterprise Edition";
                    break;
                case 6:
                    sTypeOs = "Business Edition";
                    break;
                case 7:
                    sTypeOs = "Windows Server Standard Edition (Desktop Experience installation)";
                    break;
                case 8:
                    sTypeOs = "Windows Server Datacenter Edition (Desktop Experience installation";
                    break;
                case 9:
                    sTypeOs = "Small Business Server Editio";
                    break;
                case 10:
                    sTypeOs = "Enterprise Server Edition";
                    break;
                case 11:
                    sTypeOs = "Starter Edition";
                    break;
                case 12:
                    sTypeOs = "Datacenter Server Core Edition";
                    break;
                case 13:
                    sTypeOs = "Standard Server Core Edition";
                    break;
                case 14:
                    sTypeOs = "Enterprise Server Core Edition";
                    break;
                case 17:
                    sTypeOs = "Web Server Edition";
                    break;
                case 19:
                    sTypeOs = "Home Server Edition";
                    break;
                case 20:
                    sTypeOs = "Storage Express Server Edition";
                    break;
                case 21:
                    sTypeOs = "Windows Storage Server Standard Edition (Desktop Experience installation)";
                    break;
                case 22:
                    sTypeOs = "Windows Storage Server Workgroup Edition (Desktop Experience installation)";
                    break;
                case 23:
                    sTypeOs = "Storage Enterprise Server Edition";
                    break;
                case 24:
                    sTypeOs = "Server For Small Business Edition";
                    break;
                case 25:
                    sTypeOs = "Small Business Server Premium Edition";
                    break;
                case 27:
                    sTypeOs = "Windows Enterprise Edition";
                    break;
                case 28:
                    sTypeOs = "Windows Ultimate Edition";
                    break;
                case 29:
                    sTypeOs = "Windows Server Web Server Edition (Server Core installation)";
                    break;
                case 36:
                    sTypeOs = "Windows Server Standard Edition without Hyper-V";
                    break;
                case 37:
                    sTypeOs = "Windows Server Datacenter Edition without Hyper-V (full installation)";
                    break;
                case 38:
                    sTypeOs = "Windows Server Enterprise Edition without Hyper-V (full installation)";
                    break;
                case 39:
                    sTypeOs = "Windows Server Datacenter Edition without Hyper-V (Server Core installation)";
                    break;
                case 40:
                    sTypeOs = "Windows Server Standard Edition without Hyper-V (Server Core installation)";
                    break;
                case 41:
                    sTypeOs = "Windows Server Enterprise Edition without Hyper-V (Server Core installation)";
                    break;
                case 42:
                    sTypeOs = "Microsoft Hyper-V Server";
                    break;
                case 43:
                    sTypeOs = "Storage Server Express Edition (Server Core installation)";
                    break;
                case 44:
                    sTypeOs = "Storage Server Standard Edition (Server Core installation)";
                    break;
                case 45:
                    sTypeOs = "Storage Server Workgroup Edition (Server Core installation)";
                    break;
                case 46:
                    sTypeOs = "Storage Server Enterprise Edition (Server Core installation)";
                    break;
                case 50:
                    sTypeOs = "Windows Server Essentials (Desktop Experience installation)";
                    break;
                case 63:
                    sTypeOs = "Small Business Server Premium (Server Core installation)";
                    break;
                case 64:
                    sTypeOs = "Windows Compute Cluster Server without Hyper-V";
                    break;
                case 97:
                    sTypeOs = "CORE_ARM";
                    break;
                case 101:
                    sTypeOs = "Windows Home";
                    break;
                case 103:
                    sTypeOs = "Windows Professional with Media Center";
                    break;
                case 104:
                    sTypeOs = "Windows Mobile";
                    break;
                case 123:
                    sTypeOs = "Windows IoT (Internet of Things) Core";
                    break;
                case 143:
                    sTypeOs = "Windows Server Datacenter Edition (Nano Server installation)";
                    break;
                case 144:
                    sTypeOs = "Windows Server Standard Edition (Nano Server installation)";
                    break;
                case 147:
                    sTypeOs = "Windows Server Datacenter Edition (Server Core installation)";
                    break;
                case 148:
                    sTypeOs = "Windows Server Standard Edition (Server Core installation)";
                    break;
                default:
                    sTypeOs = iNumber.ToString();
                    break;
            }
            return sTypeOs;
        }

        private string CheckTypeOS(int iNumber) //Type host
        {
            string sTypeOs = "";
            switch (iNumber)
            {
                case 1:
                    sTypeOs = "Workstation";
                    break;
                case 2:
                    sTypeOs = "Domain Controller";
                    break;
                case 3:
                    sTypeOs = "Server";
                    break;
                default:
                    sTypeOs = iNumber.ToString();
                    break;
            }
            return sTypeOs;
        }

        private string CheckTypeMemory(int iNumber) //Type memory
        {
            string sTypeMemory = "";
            switch (iNumber)
            {
                case 0:
                    sTypeMemory = iNumber.ToString();
                    break;
                case 1:
                    sTypeMemory = "Other";
                    break;
                case 2:
                    sTypeMemory = "DRAM";
                    break;
                case 3:
                    sTypeMemory = "Synchronous DRAM";
                    break;
                case 4:
                    sTypeMemory = "Cache DRAM";
                    break;
                case 5:
                    sTypeMemory = "EDO";
                    break;
                case 6:
                    sTypeMemory = "EDRAM";
                    break;
                case 7:
                    sTypeMemory = "VRAM";
                    break;
                case 8:
                    sTypeMemory = "SRAM";
                    break;
                case 9:
                    sTypeMemory = "RAM";
                    break;
                case 10:
                    sTypeMemory = "ROM";
                    break;
                case 11:
                    sTypeMemory = "Flash";
                    break;
                case 12:
                    sTypeMemory = "EEPROM";
                    break;
                case 13:
                    sTypeMemory = "FEPROM";
                    break;
                case 14:
                    sTypeMemory = "EPROM";
                    break;
                case 15:
                    sTypeMemory = "CDRAM";
                    break;
                case 16:
                    sTypeMemory = "3DRAM";
                    break;
                case 17:
                    sTypeMemory = "SDRAM";
                    break;
                case 18:
                    sTypeMemory = "SGRAM";
                    break;
                case 19:
                    sTypeMemory = "RDRAM";
                    break;
                case 20:
                    sTypeMemory = "DDR";
                    break;
                case 21:
                    sTypeMemory = "DDR2";
                    break;
                case 22:
                    sTypeMemory = "DDR2 FB-DIMM";
                    break;
                case 23:
                    sTypeMemory = "";
                    break;
                case 24:
                    sTypeMemory = "DDR3";
                    break;
                case 25:
                    sTypeMemory = "FBD2";
                    break;
                case 26:
                    sTypeMemory = "DDR3";
                    break;
                case 27:
                    sTypeMemory = "FBD2";
                    break;
                default:
                    sTypeMemory = iNumber.ToString();
                    break;
            }
            return sTypeMemory;
        }

        private string CheckFormFactorMemory(int iNumber) //Formfactor memory
        {
            string sTypeMemory = "";
            switch (iNumber)
            {
                case 0:
                    sTypeMemory = iNumber.ToString();
                    break;
                case 1:
                    sTypeMemory = "Other";
                    break;
                case 2:
                    sTypeMemory = "SIP";
                    break;
                case 3:
                    sTypeMemory = "DIP";
                    break;
                case 4:
                    sTypeMemory = "ZIP";
                    break;
                case 5:
                    sTypeMemory = "SOJ";
                    break;
                case 6:
                    sTypeMemory = "Proprietary";
                    break;
                case 7:
                    sTypeMemory = "SIMM";
                    break;
                case 8:
                    sTypeMemory = "DIMM";
                    break;
                case 9:
                    sTypeMemory = "TSOP";
                    break;
                case 10:
                    sTypeMemory = "PGA";
                    break;
                case 11:
                    sTypeMemory = "RIMM";
                    break;
                case 12:
                    sTypeMemory = "SODIMM";
                    break;
                case 13:
                    sTypeMemory = "SRIMM";
                    break;
                case 14:
                    sTypeMemory = "SMD";
                    break;
                case 15:
                    sTypeMemory = "SSMP";
                    break;
                case 16:
                    sTypeMemory = "QFP";
                    break;
                case 17:
                    sTypeMemory = "TQFP";
                    break;
                case 18:
                    sTypeMemory = "SOIC";
                    break;
                case 19:
                    sTypeMemory = "LCC";
                    break;
                case 20:
                    sTypeMemory = "PLCC";
                    break;
                case 21:
                    sTypeMemory = "BGA";
                    break;
                case 22:
                    sTypeMemory = "FPBGA";
                    break;
                case 23:
                    sTypeMemory = "LGA";
                    break;
                default:
                    sTypeMemory = iNumber.ToString();
                    break;
            }
            return sTypeMemory;
        }
        
        private void FormClosedFull()
        {
            GC.Collect();
            Application.Exit();
        }
    }
}
