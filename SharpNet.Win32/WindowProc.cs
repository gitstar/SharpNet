using SharpNet.Log;
using SharpNet.User32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpNet.Win32
{
    public class WindowProc
    {
        /// <summary>
        /// Get Verion of OS.
        /// </summary>
        /// <returns></returns>
        public static string GetOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        /// <summary>
        /// 어미 윈도우 찾기
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static IntPtr FindMainWindow(IntPtr ptr)
        {
            IntPtr lastWindowHandle = IntPtr.Zero;

            while (true)
            {
                IntPtr temp = WinAPI.GetParent(ptr);
                if (temp.Equals(IntPtr.Zero)) break;
                lastWindowHandle = temp;
            }

            return lastWindowHandle;
        }

        /// <summary>
        /// 해당한 프로세스를  찾는다.
        /// </summary>
        /// <param name="strWindowName"></param>
        /// <returns></returns>
        public static IntPtr FindGameWindow(string strWindowName)
        {
            IntPtr hWnd = IntPtr.Zero;

            List<Process> plist = Process.GetProcesses().ToList();

            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(strWindowName))
                {
                    hWnd = pList.MainWindowHandle;
                    WinAPI.SetWindowPos(pList.MainWindowHandle,WinAPI.HWND_TOPMOST, 0, 0, 0, 0, WinAPI.SWP_NOMOVE | WinAPI.SWP_NOSIZE);
                    break;
                }
            }
            return hWnd;
        }

        /// <summary>
        /// 자식 윈도우 찾기
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="strtiltle"></param>
        /// <returns></returns>
        public static IntPtr FindChildWindow(IntPtr ptr, string strtiltle)
        {
             IntPtr handle = WinAPI.FindWindowEx(ptr, 0, null, strtiltle);

            //IntPtr handle =WinAPI.FindWindowEx(ptr, 0, "Internet Explorer_Server", null);
            //IntPtr handle = FindWindowEx(ptr, IntPtr.Zero, strtiltle, null);

            return handle;
        }

        /// <summary>
        /// 윈도우 핸들의 부모 핸들의 caption 얻기
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static string GetCaptionOfWindow(IntPtr hwnd)
        {
            string caption = "";
            StringBuilder windowText = null;
            try
            {
                int max_length = WinAPI.GetWindowTextLength(hwnd);
                windowText = new StringBuilder("", max_length + 5);
                WinAPI.GetWindowText(hwnd, windowText, max_length + 2);

                if (!String.IsNullOrEmpty(windowText.ToString()) && !String.IsNullOrWhiteSpace(windowText.ToString()))
                    caption = windowText.ToString();
            }
            catch (Exception ex)
            {
                caption = ex.Message;
            }
            finally
            {
                windowText = null;
            }
            return caption;
        }

        /// <summary>
        /// 해당 좌표 마우스 클릭
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="point"></param>
        public static void MouseClick(IntPtr hWnd, POINT point)
        {
            WinAPI.SetCursor(hWnd);
            WinAPI.SetCursorPos((int)point.X, (int)point.Y);//마우스 이동
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0); //마우스다운
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);//마우스 업
            Thread.Sleep(100);
        }

        /// <summary>
        /// 프로세스 종료
        /// </summary>
        /// <param name="exeName"></param>
        /// <returns></returns>
        public static bool KillProcess(string exeName)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(exeName))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("killProcess", ex.ToString());
                return false;
            }
            return true;
        }




        /// <summary>
        /// 프로그램의 빌드버전 얻기
        /// </summary>
        /// <returns></returns>
        public static string AppVerion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            return version;
        }

        /// <summary>
        /// 프로그램사용제한 날자설정
        /// </summary>
        /// <returns></returns>
        public static int RemainValidDate(DateTime dateTime)
        {
            DateTime curDate = DateTime.Now;
            //DateTime limDate = new DateTime(2018, 11, 30);
            int maxDate = Convert.ToInt32((dateTime - curDate).TotalDays);

            return maxDate;
        }

        /// <summary>
        /// 관리자모드로 플 실행
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="param"></param>
        public static void ExecuteAsAdmin(string fileName, string param = null)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;

            if (param != null)
                proc.StartInfo.Arguments = param;

            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                proc.StartInfo.Verb = "runas";
            }
            proc.Start();
        }
    }
}
