using SharpNet.Log;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace SharpNet.Win32
{
    public class Injection
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId);


        //for detach dll
        [DllImport("kernel32")]
        public static extern IntPtr CreateToolhelp32Snapshot(Int32 dwFlags, Int32 th32ProcessID);
        [DllImport("kernel32")]
        public static extern void CloseHandle(IntPtr hObject);
        [DllImport("kernel32")]
        public static extern Int32 Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 pe32);

        [DllImport("kernel32")]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32W lpme);

        [DllImport("kernel32")]
        public static extern Int32 Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 pe32);

        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32W lpme);

        public const Int32 MAX_PATH = 260;
        public const Int32 TH32CS_SNAPPROCESS = 2;

        const int TH32CS_SNAPMODULE = 0x00000008;
        //const int PF_ERR_UNKNOWN = -1;
        const int PF_ERR_NOERROR = 0;
        const int PF_ERR_GET_SNAPSHOT = 2;
        const int PF_ERR_FIND_FIRST_MOD = 3;
        const int PF_ERR_FIND_NEXT_MOD = 4;
        const int PF_ERR_SYS_PROCESS = 5;
        const int PF_ERR_PROC_INFO = 6;
        //const int PF_ERR_NOT_FOUND = 7;

        // privileges
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;

        // used for memory allocation
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public Int32 dwSize;
            public Int32 cntUsage;
            public Int32 th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public Int32 th32ModuleID;
            public Int32 cntThreads;
            public Int32 th32ParentProcessID;
            public Int32 pcPriClassBase;
            public Int32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public String szExeFile;

            public static Int32 Size
            {
                get { return Marshal.SizeOf(typeof(PROCESSENTRY32)); }
            }
        }

        public struct MODULEENTRY32W
        {
            public Int32 dwSize;
            public uint th32ModuleID;
            public uint th32ProcessID;
            public uint GlblcntUsage;
            public uint ProccntUsage;
            public IntPtr modBaseAddr;
            public uint modBaseSize;
            public IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;

            public static Int32 Size
            {
                get { return Marshal.SizeOf(typeof(MODULEENTRY32W)); }
            }
        }


        public static bool isInjected = false;
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        static bool is64BitProcess = (IntPtr.Size == 8);
        static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        private static bool inject(string dllPath, Process tProcess)
        {
            try
            {
                Process targetProcess = tProcess;
                string dllName = dllPath;

                // the target process
                // geting the handle of the process - with required privileges
                IntPtr procHandle = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, false, targetProcess.Id);
                // name of the dll we want to inject
                // alocating some memory on the target process - enough to store the name of the dll
                // and storing its address in a pointer
                IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                // writing the name of the dll there
                UIntPtr bytesWritten;
                WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);
                // searching for the address of LoadLibraryA and storing it in a pointer
                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                // creating a thread that will call LoadLibraryA with allocMemAddress as argument
                CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                SharpLog.Error("inject", ex.ToString());
                return false;
            }

            return true;
        }

        private static int eject(string dllPath, Process tProcess)
        {
            int w_nRet = PF_ERR_NOERROR;
            IntPtr w_hProcess = IntPtr.Zero;
            IntPtr hSnapshot = IntPtr.Zero;
            IntPtr w_hThread = IntPtr.Zero;

            //PROCESSENTRY32 ModInfo = new PROCESSENTRY32();
            //ModInfo.dwSize = PROCESSENTRY32.Size;

            MODULEENTRY32W moduleEntry = new MODULEENTRY32W();
            moduleEntry.dwSize = MODULEENTRY32W.Size;
            bool bFound = false;


            //해당 dll 이 인젝되여 있는가? 있다면 어디에 있는가?
            hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, tProcess.Id);
            if (hSnapshot == IntPtr.Zero)
            {
                w_nRet = PF_ERR_GET_SNAPSHOT;
                goto L_EXIT;
            }

            // if (Process32First(hSnapshot, ref ModInfo) == 0)
            if (!Module32First(hSnapshot, ref moduleEntry))
            {
                CloseHandle(hSnapshot);
                w_nRet = PF_ERR_FIND_FIRST_MOD;
                goto L_EXIT;
            }

            do
            {
                if (moduleEntry.szModule == dllPath)
                {
                    bFound = true;
                    break;
                }
            } while (Module32Next(hSnapshot, ref moduleEntry));


            CloseHandle(hSnapshot);
            if (bFound == false)
            {
                try
                {
                    // CloseHandle(hSnapshot);
                    w_nRet = PF_ERR_FIND_NEXT_MOD;
                }
                catch (Exception)
                {

                }

                goto L_EXIT;
            }

            w_hProcess = OpenProcess(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ, true, tProcess.Id);
            if (w_hProcess == IntPtr.Zero)
            {
                w_nRet = PF_ERR_SYS_PROCESS;
                goto L_EXIT;
            }

            // Attach 시킨다.
            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "FreeLibrary");
            //원격방식으로함수를싱행시킨다.
            w_hThread = CreateRemoteThread(w_hProcess, IntPtr.Zero, 0, loadLibraryAddr, (IntPtr)moduleEntry.modBaseAddr, 0, IntPtr.Zero);
            if (w_hThread == IntPtr.Zero)
            {
                w_nRet = PF_ERR_PROC_INFO;
                goto L_EXIT;
            }

        L_EXIT:

            if (w_hThread != IntPtr.Zero)
            {
                CloseHandle(w_hThread);
            }
            if (w_hProcess != IntPtr.Zero)
            {
                CloseHandle(w_hProcess);
            }

            return w_nRet;
        }

        public static Process InjectDll(string dllName)
        {
            string rawDLL = String.Empty;
            if (is64BitOperatingSystem)
            {
                rawDLL = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), dllName);
            }
            else
            {
                rawDLL = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), dllName);
            }
            // Execution of injection
            // var procs = Process.GetProcesses();
            Process curProcess = Process.GetProcessesByName("Poker")[0];
            if (curProcess == null)
                return null;

            if (inject(rawDLL, curProcess))
            {
                isInjected = true;
                return curProcess;
            }
            else
            {
                isInjected = false;
                return null;
            }
        }

        public static int EjectDll(string dllName, Process process)
        {
            string rawDLL = dllName;
            int nResult = eject(rawDLL, process);
            isInjected = false;

            return nResult;
        }
        public static Boolean isInjectedAlready()
        {
            if (isInjected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
