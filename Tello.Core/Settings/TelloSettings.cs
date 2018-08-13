using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace Tello.Core
{
    [JsonObject]
    public class TelloSettings
    {
        public const string CONFIG_FILE_NAME = "tellosettings.json";

        [JsonConstructor]
        public TelloSettings()
        {
            Network = new NetworkSetting();
        }

        #region Properties
        [JsonProperty]
        public INetworkSetting Network { get; }

        #endregion


        public static TelloSettings Load(string directory)
        {

            if (!Directory.Exists(directory))
            {
                throw new ArgumentException("The Given directory does not exists.", "directory");
            }

            var file = Path.Combine(directory, CONFIG_FILE_NAME);
            if (!File.Exists(file))
            {
                throw new ArgumentException("The given directory does not contain the Tello setting file.", "directory");
            }

            var config = File.ReadAllText(file, Encoding.UTF8);


            return JsonConvert.DeserializeObject<TelloSettings>(config, GetSerializerSettings());
        }

        public static bool Save(TelloSettings telloSettings, string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentNullException(file);
            }

            var config = JsonConvert.SerializeObject(telloSettings, GetSerializerSettings());
            File.WriteAllText(file, config, Encoding.UTF8);
            return true;
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            foreach (var converter in NetworkSetting.Converters)
            {
                settings.Converters.Add(converter);
            }
            settings.TypeNameHandling = TypeNameHandling.Auto;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            return settings;
        }
    }

    [JsonObject]
    public class NetworkSetting : INetworkSetting
    {
        public static IReadOnlyList<JsonConverter> Converters { get; }

        static NetworkSetting()
        {
            Converters = new List<JsonConverter>()
            {
                new IPAddressConverter(),
                new IPEndPointConverter()
            };
        }

        [JsonConstructor]
        public NetworkSetting()
        {
            SSID = Password = "";
            IPv4 = new IPEndPoint(IPAddress.Broadcast, 8889);
            AutoReconnect = true;
            VideoStreamPort = 6038;
        }

        [JsonProperty]
        public string WiFiAdapterName { get; set; }

        [JsonProperty]
        public string SSID { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public IPEndPoint IPv4 { get; set; }

        [JsonProperty]
        public int VideoStreamPort { get; set; }

        [JsonProperty]
        public bool AutoReconnect { get; set; }



        public void Load(INetworkSetting network)
        {
            SSID = network.SSID;
            Password = network.Password;
            IPv4 = network.IPv4;
            AutoReconnect = network.AutoReconnect;
        }



        #region JsonConverter
        class IPAddressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPAddress));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return IPAddress.Parse((string)reader.Value);
            }
        }

        class IPEndPointConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPEndPoint));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                IPEndPoint ep = (IPEndPoint)value;
                JObject jo = new JObject();
                jo.Add("Address", JToken.FromObject(ep.Address, serializer));
                jo.Add("Port", ep.Port);
                jo.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                IPAddress address = jo["Address"].ToObject<IPAddress>(serializer);
                int port = (int)jo["Port"];
                return new IPEndPoint(address, port);
            }
        }

        #endregion
    }

}
