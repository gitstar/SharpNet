using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Http
{
    public class WRequest
    { 
        public WRequest()
        {

        }

        public virtual CookieCollection GetWebCookie(string url, string strHost, string strCookie = null, string contentType = "application/x-www-form-urlencoded")
        {
            CookieCollection Cookies;

            var httpWebRequest = CreateHttpWebRequest(url, "Get", contentType);
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Host = strHost;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
            myWebHeaderCollection.Add("Accept-Encoding:gzip, deflate");
            myWebHeaderCollection.Add("Accept-Language", "en-US,en;q=0.9,ko;q=0.8");
            myWebHeaderCollection.Add("Cache-Control", "max-age=0");
            myWebHeaderCollection.Add("Upgrade-Insecure-Requests", "1");


            if (strCookie.IsNotEmptyOrWhiteSpace())
            {
                CookieContainer cookiecontainer = new CookieContainer();
                string[] cookies = strCookie.Split(';');
                foreach (string cookie in cookies)
                    cookiecontainer.SetCookies(new Uri(url), cookie);
                httpWebRequest.CookieContainer = cookiecontainer;
            }
            else
            {
                CookieContainer cookieJar = new CookieContainer();
                httpWebRequest.CookieContainer = cookieJar;
            }

            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            Cookies = response.Cookies;
            //using (Stream responseStream = response.GetResponseStream())
            //{
            //    // Store Cookies
            //    Cookies = response.Cookies;
            //}

            return Cookies;
        }
        /// <summary>
        /// synchronous web response with getmode
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strHost"></param>
        /// <param name="referalSite"></param>
        /// <param name="strCookie"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public virtual string GetWebResponse(string url, string strHost, string referalSite, string strOrigin = "", string strCookie = null, string contentType = "application/x-www-form-urlencoded")
        {
            var httpWebRequest = CreateHttpWebRequest(url, "Get", contentType);
            httpWebRequest.Accept = "*/*";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Host = strHost;
            httpWebRequest.Referer = referalSite;
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9,ko;q=0.8");
            httpWebRequest.Headers.Add("Origin", strOrigin);
            httpWebRequest.Headers.Add("Sec-Fetch-Dest", "document");
            httpWebRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
            httpWebRequest.Headers.Add("Sec-Fetch-Site", "none");
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (strCookie.IsNotEmptyOrWhiteSpace())
            {
                CookieContainer cookiecontainer = new CookieContainer();
                string[] cookies = strCookie.Split(';');
                foreach (string cookie in cookies)
                    cookiecontainer.SetCookies(new Uri(url), cookie);
                httpWebRequest.CookieContainer = cookiecontainer;
            }
            else
            {
                CookieContainer cookieJar = new CookieContainer();
                httpWebRequest.CookieContainer = cookieJar;
            }

            Task<WebResponse> responseTask = Task.Factory.FromAsync<WebResponse>(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, null);
            return  GetResponseText(responseTask.Result.GetResponseStream());
           
        }


        /// <summary>
        /// synchronous web response with postmode
        /// </summary>
        /// <param name="url"></param>
        /// <param name="host"></param>
        /// <param name="referhost"></param>
        /// <param name="postParameters"></param>
        /// <param name="strOrigin"></param>
        /// <param name="strCookies"></param>
        /// <param name="contentType"></param>
        /// <param name="isEucKR"></param>
        /// <returns></returns>
        public virtual string PostWebResponse(string url, string host, string referhost, NameValueCollection postParameters, 
                  string strOrigin,
                  string strCookies = null,
                  string contentType = "application/x-www-form-urlencoded",
                  bool isEucKR = false)
        {
            HttpWebRequest httpWebRequest = CreateHttpWebRequest(url, "POST", "application/x-www-form-urlencoded");
            byte[] requestBytes = GetRequestBytes(postParameters);
            httpWebRequest.ContentLength = requestBytes.Length;
            httpWebRequest.Host = host;
            httpWebRequest.Referer = referhost;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Accept = "*.*";
            WebHeaderCollection headers = httpWebRequest.Headers;
            headers.Add("Accept-Encoding:gzip, deflate");
            headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.7,ko;q=0.3");
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Origin", strOrigin);
            headers.Add("Upgrade-Insecure-Requests", "1");
            headers.Add("Sec-Fetch-Dest", "document");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-Fetch-Mode", "navigate");
            headers.Add("Sec-Fetch-User", "?1");
            headers.Add("X_REQUESTED_WITH", "XMLHttpRequest");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
           
            if(strCookies.IsNotEmptyOrWhiteSpace())
            {
                CookieContainer cookiecontainer = new CookieContainer();
                string[] cookies = strCookies.Split(';');
                foreach (string cookie in cookies)
                    cookiecontainer.SetCookies(new Uri(url), cookie);

                httpWebRequest.CookieContainer = cookiecontainer;
            }
            
            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
            }

            Task<WebResponse> responseTask = Task.Factory.FromAsync<WebResponse>(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, null);
            return GetResponseText(responseTask.Result.GetResponseStream());
        }





        /// <summary>
        /// This method does an Http POST sending any post parameters to the url provided
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strhost"></param>
        /// <param name="refhost"></param>
        /// <param name="postParameters"></param>
        /// <param name="strOrigin"></param>
        /// <param name="responseCallback"></param>
        /// <param name="strCookies"></param>
        /// <param name="state"></param>
        /// <param name="contentType"></param>
        public virtual void PostAsync(string url, string strhost, string refhost,  NameValueCollection postParameters, string strOrigin, 
          Action<HttpWebRequestCallbackState> responseCallback, string strCookies, object state = null,
          string contentType = "application/x-www-form-urlencoded")
        {
            var httpWebRequest = CreateHttpWebRequest(url, "POST", contentType);
            var requestBytes = GetRequestBytes(postParameters);
            httpWebRequest.ContentLength = requestBytes.Length;
            httpWebRequest.Host = strhost; //"mypage.netmarble.net";
            httpWebRequest.Referer = refhost; //"http://mypage.netmarble.net/memo/popup/memoWrite.asp";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.87 Safari/537.36";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Accept = "*.*";
            WebHeaderCollection headers = httpWebRequest.Headers;
            headers.Add("Accept-Encoding:gzip, deflate");
            headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.7,ko;q=0.3");
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Origin", strOrigin);
            headers.Add("Upgrade-Insecure-Requests", "1");
            headers.Add("Sec-Fetch-Dest", "document");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-Fetch-Mode", "navigate");
            headers.Add("Sec-Fetch-User", "?1");
            headers.Add("X_REQUESTED_WITH", "XMLHttpRequest");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
           // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;

            CookieContainer cookiecontainer = new CookieContainer();
            string[] cookies = strCookies.Split(';');
            foreach (string cookie in cookies)
                cookiecontainer.SetCookies(new Uri(url), cookie);
            httpWebRequest.CookieContainer = cookiecontainer;

            httpWebRequest.BeginGetRequestStream(BeginGetRequestStreamCallback,
              new HttpWebRequestAsyncState()
              {
                  RequestBytes = requestBytes,
                  HttpWebRequest = httpWebRequest,
                  ResponseCallback = responseCallback,
                  State = state
              });
        }

        /// <summary>
        /// This method does an Http GET to the provided url and calls the responseCallback delegate
        /// providing it with the response returned from the remote server.
        /// </summary>
        /// <param name="url">The url to make an Http GET to</param>
        /// <param name="responseCallback">The callback delegate that should be called when the response returns from the remote server</param>
        /// <param name="state">Any state information you need to pass along to be available in the callback method when it is called</param>
        /// <param name="contentType">The Content-Type of the Http request</param>
        public virtual void GetAsync(string url, Action<HttpWebRequestCallbackState> responseCallback,
            object state = null, string contentType = "application/x-www-form-urlencoded")
        {
            var httpWebRequest = CreateHttpWebRequest(url, "GET", contentType);

            httpWebRequest.BeginGetResponse(BeginGetResponseCallback,
              new HttpWebRequestAsyncState()
              {
                  HttpWebRequest = httpWebRequest,
                  ResponseCallback = responseCallback,
                  State = state
              });
        }


        /// <summary>
        /// This method does an Http Post asyncronous task to the provided url and calls the responseCallback delegate
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strHost"></param>
        /// <param name="strReferer"></param>
        /// <param name="postParameters"></param>
        /// <param name="strOrigin"></param>
        /// <param name="responseCallback"></param>
        /// <param name="strCookies"></param>
        /// <param name="state"></param>
        /// <param name="contentType"></param>
        public virtual void PostAsyncTask(string url, string strHost, string strReferer, byte[] postParameters, string strOrigin,
          Action<HttpWebRequestCallbackState> responseCallback, string strCookies,  object state = null,
          string contentType = "application/x-www-form-urlencoded; charset=UTF-8")
        {
            var httpWebRequest = CreateHttpWebRequest(url, "POST", contentType);
            //var requestBytes = GetRequestBytes(postParameters, isEucKR);
            var requestBytes = postParameters;
            httpWebRequest.ContentLength = requestBytes.Length;
            httpWebRequest.Host = strHost;
            httpWebRequest.Referer = strReferer;
            httpWebRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.113 Safari/537.36";
            httpWebRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9,ko;q=0.8");
            WebHeaderCollection headers = httpWebRequest.Headers;
            headers.Add("Accept-Encoding:gzip, deflate");
            headers.Add("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.7,ko;q=0.3");
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Origin", strOrigin);
            headers.Add("Upgrade-Insecure-Requests", "1");
            headers.Add("Sec-Fetch-Dest", "document");
            headers.Add("Sec-Fetch-Site", "same-origin");
            headers.Add("Sec-Fetch-Mode", "navigate");
            headers.Add("Sec-Fetch-User", "?1");
            headers.Add("X_REQUESTED_WITH", "XMLHttpRequest");

            CookieContainer cookiecontainer = new CookieContainer();
            string[] cookies = strCookies.Split(';');
            foreach (string cookie in cookies)
                cookiecontainer.SetCookies(new Uri(url), cookie);
            httpWebRequest.CookieContainer = cookiecontainer;


            var asyncState = new HttpWebRequestAsyncState()
            {
                RequestBytes = requestBytes,
                HttpWebRequest = httpWebRequest,
                ResponseCallback = responseCallback,
                State = state
            };

            Task.Factory.FromAsync<Stream>(httpWebRequest.BeginGetRequestStream,
              httpWebRequest.EndGetRequestStream, asyncState, TaskCreationOptions.None)
              .ContinueWith<HttpWebRequestAsyncState>(task =>
              {
                  var asyncState2 = (HttpWebRequestAsyncState)task.AsyncState;
                  using (var requestStream = task.Result)
                  {
                      requestStream.Write(asyncState2.RequestBytes, 0, asyncState2.RequestBytes.Length);
                  }
                  return asyncState2;
              })
              .ContinueWith(task =>
              {
                  var httpWebRequestAsyncState2 = (HttpWebRequestAsyncState)task.Result;
                  var hwr2 = httpWebRequestAsyncState2.HttpWebRequest;
                  Task.Factory.FromAsync<WebResponse>(hwr2.BeginGetResponse,
              hwr2.EndGetResponse, httpWebRequestAsyncState2, TaskCreationOptions.None)
              .ContinueWith(task2 =>
              {
                  WebResponse webResponse = null;
                  Stream responseStream = null;
                  try
                  {
                      var asyncState3 = (HttpWebRequestAsyncState)task2.AsyncState;
                      webResponse = task2.Result;
                      responseStream = webResponse.GetResponseStream();
                      responseCallback(new HttpWebRequestCallbackState(responseStream, asyncState3));
                  }
                  finally
                  {
                      if (responseStream != null)
                          responseStream.Close();
                      if (webResponse != null)
                          webResponse.Close();
                  }
              });
              });
        }

        public virtual void PostUploadAsyncTask(string url, byte[] data,
          Action<HttpWebRequestCallbackState> responseCallback, string strCookies, string strHost, string strReferer, string strAccept, object state = null, bool isEucKR = true,
          string contentType = "application/x-www-form-urlencoded; charset=UTF-8")
        {
            var httpWebRequest = CreateHttpWebRequest(url, "POST", contentType);

            httpWebRequest.ContentLength = data.Length;
            httpWebRequest.Host = strHost;
            httpWebRequest.Referer = strReferer;
            //httpWebRequest.Accept = strAccept;
            //httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //httpWebRequest.UserAgent = "UnityPlayer/2018.4.10f1 (UnityWebRequest/1.0, libcurl/7.52.0-DEV)";

            //  httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

            CookieContainer cookiecontainer = new CookieContainer();
            //string[] cookies = strCookies.Split(';');
            //foreach (string cookie in cookies)
            //    cookiecontainer.SetCookies(new Uri(url), cookie);
            //httpWebRequest.CookieContainer = cookiecontainer;

            //Stream rs = httpWebRequest.GetRequestStream();
            //rs.Write(data, 0, data.Length);
            //rs.Close();

            var asyncState = new HttpWebRequestAsyncState()
            {
                RequestBytes = data,
                HttpWebRequest = httpWebRequest,
                ResponseCallback = responseCallback,
                State = state
            };

            Task.Factory.FromAsync<Stream>(httpWebRequest.BeginGetRequestStream,
              httpWebRequest.EndGetRequestStream, asyncState, TaskCreationOptions.None)
              .ContinueWith<HttpWebRequestAsyncState>(task =>
              {
                  var asyncState2 = (HttpWebRequestAsyncState)task.AsyncState;
                  using (var requestStream = task.Result)
                  {
                      requestStream.Write(asyncState2.RequestBytes, 0, asyncState2.RequestBytes.Length);
                  }
                  return asyncState2;
              })
              .ContinueWith(task =>
              {
                  var httpWebRequestAsyncState2 = (HttpWebRequestAsyncState)task.Result;
                  var hwr2 = httpWebRequestAsyncState2.HttpWebRequest;
                  Task.Factory.FromAsync<WebResponse>(hwr2.BeginGetResponse,
              hwr2.EndGetResponse, httpWebRequestAsyncState2, TaskCreationOptions.None)
              .ContinueWith(task2 =>
              {
                  WebResponse webResponse = null;
                  Stream responseStream = null;
                  try
                  {
                      var asyncState3 = (HttpWebRequestAsyncState)task2.AsyncState;
                      webResponse = task2.Result;
                      responseStream = webResponse.GetResponseStream();
                      responseCallback(new HttpWebRequestCallbackState(responseStream, asyncState3));
                  }
                  finally
                  {
                      if (responseStream != null)
                          responseStream.Close();
                      if (webResponse != null)
                          webResponse.Close();
                  }
              });
              });
        }

        public virtual void GetAsyncTask(string url, string strHost,string refhost,
          Action<HttpWebRequestCallbackState> responseCallback,string strCookies,object state = null,
          string contentType = "application/x-www-form-urlencoded")
        {
            try
            {
                HttpWebRequest httpWebRequest = CreateHttpWebRequest(url, "GET", contentType);
                httpWebRequest.Host = strHost;
                httpWebRequest.Referer = refhost;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                WebHeaderCollection headers = httpWebRequest.Headers;
                headers.Add("Accept-Encoding:gzip, deflate");
                headers.Add("Accept-Language", "en-US,en;q=0.9,ko;q=0.8");
                headers.Add("Upgrade-Insecure-Requests", "1");
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
               // httpWebRequest.Timeout = 3000;
                CookieContainer cookieContainer = new CookieContainer();
                if (strCookies.IsNotEmptyOrWhiteSpace())
                {
                    string str = strCookies;
                    char[] chArray = new char[1] { ';' };
                    foreach (string cookieHeader in str.Split(chArray))
                        cookieContainer.SetCookies(new Uri(url), cookieHeader);
                    httpWebRequest.CookieContainer = cookieContainer;
                }

                Task.Factory.FromAsync<WebResponse>(new Func<AsyncCallback, object, IAsyncResult>(((WebRequest)httpWebRequest).BeginGetResponse), new Func<IAsyncResult, WebResponse>(((WebRequest)httpWebRequest).EndGetResponse), (object)null).ContinueWith((Action<Task<WebResponse>>)(task =>
                {
                    WebResponse webResponse = (WebResponse)null;
                    Stream stream = (Stream)null;
                    try
                    {
                        webResponse = task.Result;
                        stream = webResponse.GetResponseStream();
                        responseCallback(new HttpWebRequestCallbackState(webResponse.GetResponseStream(), state));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally
                    {
                        stream?.Close();
                        webResponse?.Close();
                    }
                }));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        HttpWebRequest CreateHttpWebRequest(string url, string httpMethod, string contentType)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = httpMethod;
            return httpWebRequest;
        }

        /// <summary>
        /// 
        /// string postData = string.Format("ownerID={0}&content={1}&saved=1", HttpUtility.UrlEncode(gamerid, Encoder.EUCKR()), HttpUtility.UrlEncode(memoContent, Encoder.EUCKR()));
        /// </summary>
        /// <param name="postParameters"></param>
        /// <returns></returns>
        public byte[] GetRequestBytes(NameValueCollection postParameters, bool isEucKR = false)
        {
            if (postParameters == null || postParameters.Count == 0)
                return new byte[0];
            var sb = new StringBuilder();
            foreach (var key in postParameters.AllKeys)
                sb.Append(key + "=" + postParameters[key] + "&");
            sb.Length = sb.Length - 1;

            if (isEucKR)
                return SharpNet.Encoder.Encoder.EUCKR().GetBytes(sb.ToString());
            else
                return Encoding.UTF8.GetBytes(sb.ToString());
        }


        void BeginGetRequestStreamCallback(IAsyncResult asyncResult)
        {
            Stream requestStream = null;
            HttpWebRequestAsyncState asyncState = null;
            try
            {
                asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
                requestStream = asyncState.HttpWebRequest.EndGetRequestStream(asyncResult);
                requestStream.Write(asyncState.RequestBytes, 0, asyncState.RequestBytes.Length);
                requestStream.Close();
                asyncState.HttpWebRequest.BeginGetResponse(BeginGetResponseCallback,
                  new HttpWebRequestAsyncState
                  {
                      HttpWebRequest = asyncState.HttpWebRequest,
                      ResponseCallback = asyncState.ResponseCallback,
                      State = asyncState.State
                  });
            }
            catch (Exception ex)
            {
                if (asyncState != null)
                    asyncState.ResponseCallback(new HttpWebRequestCallbackState(ex));
                else
                    throw new Exception(ex.ToString());
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
            }
        }

        void BeginGetResponseCallback(IAsyncResult asyncResult)
        {
            WebResponse webResponse = null;
            Stream responseStream = null;
            HttpWebRequestAsyncState asyncState = null;
            try
            {
                asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
                webResponse = asyncState.HttpWebRequest.EndGetResponse(asyncResult);
                responseStream = webResponse.GetResponseStream();
                var webRequestCallbackState = new HttpWebRequestCallbackState(responseStream, asyncState.State);
                asyncState.ResponseCallback(webRequestCallbackState);
                responseStream.Close();
                responseStream = null;
                webResponse.Close();
                webResponse = null;
            }
            catch (Exception ex)
            {
                //SharpLog.Error("BeginGetResponseCallback", ex.ToString());
                if (asyncState != null)
                    asyncState.ResponseCallback(new HttpWebRequestCallbackState(ex));
                else
                    throw new Exception(ex.ToString());
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
                if (webResponse != null)
                    webResponse.Close();
            }
        }

        /// <summary>
        /// If the response from a remote server is in text form
        /// you can use this method to get the text from the ResponseStream
        /// This method Disposes the stream before it returns
        /// </summary>
        /// <param name="responseStream">The responseStream that was provided in the callback delegate's HttpWebRequestCallbackState parameter</param>
        /// <returns></returns>
        string GetResponseText(Stream responseStream)
        {
            using (var reader = new StreamReader(responseStream, SharpNet.Encoder.Encoder.UTF8(), true))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// get 방식으로 api 배팅한다
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string Get(string uri)
        {
            try
            {
                //Global.SetFileLogs(uri);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Accept = "application/json";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
               
            }
        }
    }

    public class HttpWebRequestAsyncState
    {
        public byte[] RequestBytes { get; set; }
        public HttpWebRequest HttpWebRequest { get; set; }
        public Action<HttpWebRequestCallbackState> ResponseCallback { get; set; }
        public Object State { get; set; }
    }

    public class HttpWebRequestCallbackState
    {
        public Stream ResponseStream { get; private set; }
        public Exception Exception { get; private set; }
        public Object State { get; set; }

        public HttpWebRequestCallbackState(Stream responseStream, object state)
        {
            ResponseStream = responseStream;
            State = state;
        }

        public HttpWebRequestCallbackState(Exception exception)
        {
            Exception = exception;
        }
    }
}
