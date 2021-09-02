using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using SharpNet.Core.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet.Parse
{
    public static class JsonParse
    {
        public static String JsonSerialize(this SocketPacket request)
        {
            string jsonData = string.Empty;

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                Converters = new[] { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" } }
            };

            jsonData = JsonConvert.SerializeObject(request, jsonSerializerSettings);

            return jsonData;
        }

        public static string JsonSerialize(this object objData)
        {
            return JsonConvert.SerializeObject(objData);
        }

        public static String JsonSerialize(this JObject objData)
        {
            string jsonData = JsonConvert.ToString(objData);

            return jsonData;
        }

        public static T JsonDeserialize<T>(this string jsonData)
        {
            T myObject = JsonConvert.DeserializeObject<T>(jsonData);

            return myObject;
        }

        public static T JsonDeserialize<T>(this object jsonData)
        {
            T myObject = JsonConvert.DeserializeObject<T>(jsonData.ToString());

            return myObject;
        }
    }
}
