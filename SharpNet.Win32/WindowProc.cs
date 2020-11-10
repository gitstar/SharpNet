using SharpNet.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpNet.Win32
{
    public class WindowProc
    {
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
                    break;
                }
            }
            return hWnd;
        }

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
