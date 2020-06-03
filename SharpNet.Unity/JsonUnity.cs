using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Unity
{
    public static class JsonUnity
    {
        public static string JsonUnityEncode(this object objData)
        {
            return JsonConvert.SerializeObject(objData);
        }

        public static string JsonUnityEncode(this JObject objData)
        {
            return objData.ToString();
        }
        public static JObject JsonUnityDecode(this string jsonData)
        {
            return JsonConvert.DeserializeObject<JObject>(jsonData);
        }
        public static T JsonUnityDecode<T>(this string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
}
