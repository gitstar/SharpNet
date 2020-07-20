using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpNet.Log
{
    public class FileLog
    {
        private static FileLog instance;
        private const int MaxCharWithinOneLine = 2000;

        public RichTextBox richTextBox { get; set; }

        public static FileLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileLog();
                }
                return instance;
            }
        }

        /// <summary>
        /// 콘셜 로그를 남긴다.
        /// </summary>
        /// <param name="logMsg"></param>
        public void ConsolLog(string logMsg)
        {
#if DEBUG
            Console.WriteLine(logMsg);
#else
    //Console.WriteLine("Mode=Release"); 
#endif
        }

        public void Debug(string methodName, string logData, string className = null, bool isEnable = true)
        {
            SharpLog.Debug(methodName, logData, className, isEnable);
        }

        public void Error(string methodName, string logData, string className = null, bool isEnable = true)
        {
            SharpLog.Error(methodName, logData, className, isEnable);
        }

        public void DisplayLog(string strResult, Color color = default(Color), bool isEnable = true)
        {
            if (!isEnable)
                return;

            string str = string.Format("{0}:<{1}>\r\n", DateTime.Now.ToString("MM/dd hh:mm:ss"), strResult);

            try
            {
                richTextBox.Invoke(new MethodInvoker(delegate ()
                {
                    if (richTextBox.Text.Length >= MaxCharWithinOneLine)
                        richTextBox.Text.Remove(0, richTextBox.Text.IndexOf('\r') + 2);

                    richTextBox.AppendText(str);
                    richTextBox.ScrollToCaret();

                    Debug("", strResult);

                }));
            }
            catch (Exception ex)
            {
                SharpLog.Fatal("DisplayLog", ex.ToString());
            }
        }

        public void DisplayLogClean()
        {
            richTextBox.Invoke(new MethodInvoker(delegate ()
            {
                richTextBox.Text = "";
            }));
        }


        //public static void AppendText(this RichTextBox box, string str, Color color)
        //{
        //    str = string.Format("{0}:<{1}>\r\n", DateTime.Now.ToString("MM/dd hh:mm:ss"), str);

        //    if (box.Text.Length >= MaxCharWithinOneLine)
        //        box.Text.Remove(0, box.Text.IndexOf('\r') + 2);

        //    box.SelectionStart = box.TextLength;
        //    box.SelectionLength = 0;

        //    box.SelectionColor = color;
        //    box.AppendText(str);
        //    box.SelectionColor = box.ForeColor;
        //}


        public static void InvokeIfRequired(Control control, MethodInvoker action)
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(action);
                }
                else
                {
                    action();
                }
            }
            catch { }
        }

    }
}
