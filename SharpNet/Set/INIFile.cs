using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpNet.Set
{
    public class INIFile
    {
        private string iniPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public INIFile(string fileName = "")
        {
            // string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string path = Environment.CurrentDirectory;

            this.iniPath = path + @"\" + fileName;  //INI 파일 위치를 생성할때 인자로 넘겨 받음
        }

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(    // GetIniValue 를 위해
            String section,
            String key,
            String def,
            StringBuilder retVal,
            int size,
            String filePath);

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(  // SetIniValue를 위해
          String section,
          String key,
          String val,
          String filePath);


        // INI 값을 읽어 온다. 
        public String GetIniValue(String Section, String Key , String Value = "")
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, Value, temp, 255, iniPath);
            return temp.ToString();
        }

        // INI 값을 셋팅
        public void SetIniValue(String Section, String Key, String Value = "")
        {
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }

    }
}
