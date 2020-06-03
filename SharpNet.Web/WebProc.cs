using mshtml;
using SharpNet.Log;
using SharpNet.Win32;
using SHDocVw;
using System;
using System.Threading;
using System.Windows.Forms;

namespace SharpNet.Web
{
    public class WebProc
    {
        private static SHDocVw.InternetExplorer ie;
        string webUrl = "https://google.com";

        public WebProc(string urlAddress = null)
        {
            ie = new SHDocVw.InternetExplorer();
            ie.DocumentComplete += Ie_DocumentComplete;
            webUrl = urlAddress;
        }

        public virtual void Ie_DocumentComplete(object pDisp, ref object URL)
        {
            //throw new NotImplementedException();
        }

        public void Initialize()
        {
            try
            {
                WindowProc.KillProcess("iexplore");
                ie = new SHDocVw.InternetExplorer();
            }
            catch (Exception ex)
            {
                SharpLog.Error("WebProcInit", ex.ToString());

            }
        }

        public object OpenWebPage(ref object url, bool isShow = true)
        {
            try
            {
                if (isShow)
                    ie.Visible = true;
                else
                    ie.Visible = false;

                ie.Navigate2(ref url);

                while (true)
                {
                    if (!isBusy())
                        break;

                    Thread.Sleep(1000);
                }

                return ie.Document;
            }
            catch (Exception ex)
            {
                SharpLog.Error("OpenWebPage", ex.ToString());
            }

            return null;

        }

        public void Close()
        {
            try
            {
                if (ie != null)
                {
                    //ie.Quit();
                    WindowProc.KillProcess("iexplore");
                }
            }
            catch (Exception ex)
            {
                SharpLog.Error("WebProcClose", ex.ToString());
            }

        }
        public bool isBusy()
        {
            if (ie.Busy || ie.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                return true;
            else
                return false;
        }

        public IntPtr GetIEHandle()
        {
            return (IntPtr)ie.HWND;
        }

        /// <summary>
        /// 일정한 시간 유지하면서 해당 url사이트가 열리는지 검사
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public SHDocVw.WebBrowser FindIE(string url)
        {
            int limit = 0;
            while (true)
            {
                var shellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.WebBrowser wb in shellWindows)
                {
                    if (!string.IsNullOrEmpty(wb.LocationURL))
                    {
                        if (wb.LocationURL.IndexOf(url, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (wb.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                            {
                                return wb;
                            }
                            limit++;
                        }
                    }
                }
                if (limit == 50)
                {
                    break;
                }
                limit++;
                Thread.Sleep(200);
            }
            return null;
        }

        /// <summary>
        /// 현재 해당 url을 가진 싸이트가 열렸는가 판정
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public SHDocVw.WebBrowser ExistIE(string url)
        {
            var shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.WebBrowser wb in shellWindows)
            {
                if (!string.IsNullOrEmpty(wb.LocationURL))
                {
                    if (wb.LocationURL.IndexOf(url, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        if (wb.ReadyState == SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                        {
                            return wb;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// htmlelement 의 x좌표
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public int GetXoffset(HtmlElement el)
        {
            //get element pos
            int xPos = el.OffsetRectangle.Left;

            //get the parents pos
            HtmlElement tempEl = el.OffsetParent;
            while (tempEl != null)
            {
                xPos += tempEl.OffsetRectangle.Left;
                tempEl = tempEl.OffsetParent;
            }

            return xPos;
        }

        /// <summary>
        ///  htmlelement 의 Y좌표
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public int GetYoffset(HtmlElement el)
        {
            //get element pos
            int yPos = el.OffsetRectangle.Top;

            //get the parents pos
            HtmlElement tempEl = el.OffsetParent;
            while (tempEl != null)
            {
                yPos += tempEl.OffsetRectangle.Top;
                tempEl = tempEl.OffsetParent;
            }

            return yPos;
        }

        /// <summary>
        ///  IHTMLElement 의 X좌표
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int FindPosX(IHTMLElement obj)
        {
            int curleft = 0;
            if (obj.offsetParent != null)
            {
                while (obj.offsetParent != null)
                {
                    curleft += obj.offsetLeft;
                    obj = obj.offsetParent;
                }
            }

            return curleft;
        }

        /// <summary>
        ///  IHTMLElement 의 Y좌표
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int FindPosY(IHTMLElement obj)
        {
            int curtop = 0;
            if (obj.offsetParent != null)
            {
                while (obj.offsetParent != null)
                {
                    curtop += obj.offsetTop;
                    obj = obj.offsetParent;
                }
            }

            return curtop;
        }
    }
}
