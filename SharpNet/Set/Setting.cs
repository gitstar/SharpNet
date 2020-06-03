using System;
using System.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SharpNet.Set
{
    public class Setting
    {
        private static readonly Lazy<Setting> _instance = new Lazy<Setting>(() => new Setting());
        //  private static readonly Lazy<RedisProvider> _redis = new Lazy<RedisProvider>(() => RedisFactoryBuilder.Instance.Build());
        public static Setting Current
        {
            get { return _instance.Value; }
        }

        public DateTime SystemDate
        {
            get { return DateTime.Now; }
        }

        public DateTime NullDate
        {
            get { return DateTime.Parse("9999-12-31"); }
        }

        public string NullString
        {
            get { return "##!!%%&&"; }
        }


        //public RedisProvider Cache
        //{
        //    get { return _redis.Value; }
        //}

        public string Location
        {
            get
            {
                return ConfigurationManager.AppSettings["Location"];
            }
        }

        public string GetMACAddress()
        {
            string result = "";
            NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            NetworkInterface[] array = allNetworkInterfaces;
            for (int i = 0; i < array.Length; i++)
            {
                NetworkInterface networkInterface = array[i];
                PhysicalAddress physicalAddress = networkInterface.GetPhysicalAddress();
                bool flag = physicalAddress != null && !physicalAddress.ToString().Equals("");
                if (flag)
                {
                    result = physicalAddress.ToString();
                    break;
                }
            }
            return result;
        }

        public string GetIPAddress()
        {
            string result;
            try
            {
                WebClient webClient = new WebClient();
                string text = webClient.DownloadString("http://ipinfo.io/ip").Trim();
                result = text;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                result = "error";
            }
            return result;
        }

        public string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
           
        }
    }
}
