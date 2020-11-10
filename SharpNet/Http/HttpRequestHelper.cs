using SharpNet.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Http
{
    public class HttpRequestHelper
    {
        public delegate void HttpGetCallBack(HttpWebRequestCallbackState webRequeststate);
        public static event HttpGetCallBack onHttpGetCallBack;

        public delegate void HttpPostCallBack(HttpWebRequestCallbackState webRequeststate);
        public static event HttpPostCallBack onHttpPostCallBack;

        WRequest wRequest;

        public HttpRequestHelper()
        {
            wRequest = new WRequest();

        }

        /// <summary>
        /// Get Async 콜백함수
        /// </summary>
        Action<HttpWebRequestCallbackState> requestGetCallBack = (param) =>
        {
            onHttpGetCallBack?.Invoke(param);
        };

        /// <summary>
        /// Post Async 콜백함수
        /// </summary>
        Action<HttpWebRequestCallbackState> requestPostCallBack = (param) =>
        {
            onHttpPostCallBack?.Invoke(param);
        };

        public void GetAsyncRequest(string pUrl, string host, string referhost, string cookies, object param, bool islog = false)
        {
            if (islog)
                SharpLog.Debug("GetAsyncRequest", pUrl);

            if (pUrl != "" && pUrl != "-1")
            {
                wRequest.GetAsyncTask(pUrl, host, referhost, requestGetCallBack, cookies,param);
            }
        }

        public void PostAsyncRequest(string pUrl, string host, string referhost, string cookies, NameValueCollection postParameters, object param )
        {
            //Global.SetFileLogs(pUrl);
            if (pUrl != "")
            {
                //string accept = "*/*";
                byte[] bytes = wRequest.GetRequestBytes(postParameters);
                wRequest.PostAsyncTask(pUrl, host, referhost, bytes,"", requestPostCallBack, cookies,  param);
            }
        }
    }
}
