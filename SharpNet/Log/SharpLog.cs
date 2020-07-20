using NLog;
using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace SharpNet.Log
{
    public static class SharpLog
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // private static Logger logger = LogManager.GetLogger("Test");
        //private static Logger logger = LogManager.CreateNullLogger();
        //logger.Log(LogLevel.Info, "Sample fatal error message, k={0}, l={1}", k, l);


        ///// <summary>
        ///// write Tracer log
        ///// </summary>
        ///// <param name="methodName"></param>
        ///// <param name="logData"></param>
        ///// <param name="className"></param>
        public static void Tracer(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Trace("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// write Debug log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="logData"></param>
        /// <param name="className"></param>
        public static void Debug(string methodName, string logData, string className = null, bool isEnable = false)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Debug("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// Write Info log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="logData"></param>
        /// <param name="className"></param>
        public static void Info(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Info("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// Write Waring Log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="logData"></param>
        /// <param name="className"></param>
        public static void Waring(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Warn("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// Write Error Log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="logData"></param>
        /// <param name="className"></param>
        public static void Error(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Error("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// Write Fatal Log
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="logData"></param>
        /// <param name="className"></param>
        public static void Fatal(string methodName, string logData, string className = null , bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Fatal("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// Write Form View Message Log (trace 사용/ UI 출력을 위하여 사용  )
        /// </summary>
        /// <param name="methodName">로그인/게임탈퇴 등.</param>
        /// <param name="logData">아디/암호/금액 등.</param>
        /// <param name="className">User/DB 등으로 표기</param>
        public static void View(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (!string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger(className);

            logger.Debug("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// //Send email log (info 사용) / 날자.시간을 포함할것
        /// </summary>
        /// <param name="methodName">충전/환전 등..</param>
        /// <param name="logData"> 날자,시간을 포함할것.</param>
        /// <param name="className"></param>
        public static void Mail(string methodName, string logData, string className = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (string.IsNullOrEmpty(className))
                logger = LogManager.GetLogger("Mail");
            else
                logger = LogManager.GetLogger(className);

            logger.Info("{0}|{1}", methodName, logData);
        }

        /// <summary>
        /// ShowMessageBox in TopMost Method
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="caption"></param>
        public static void ShowMessageBox(string desc, string caption = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (string.IsNullOrEmpty(caption))
                MessageBox.Show(new Form { TopMost = true }, desc);
            else
                MessageBox.Show(new Form { TopMost = true }, desc, caption);
        }

        /// <summary>
        /// log를 남기고 메세지 보인다.
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="caption"></param>
        public static void ShowMessageBoxWithLog(string desc, string caption = null, bool isEnable = true)
        {
            if (!isEnable)
                return;

            if (string.IsNullOrEmpty(caption))
            {
                Debug("SharpLog", desc);
                MessageBox.Show(new Form { TopMost = true }, desc);
            }
            else
            {
                Debug(caption, desc);
                MessageBox.Show(new Form { TopMost = true }, desc, caption);
            }
        }
    }
}
