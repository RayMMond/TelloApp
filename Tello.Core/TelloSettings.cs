using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Tello.Core
{
    [JsonObject]
    public class TelloSettings : ITelloSettings
    {
        const string CONFIG_FILE_NAME = "tellosettings.json";

        private ILogger logger;
        private string workingDirectory;


        public TelloSettings(ILoggerFactory loggerFactory,
            INetworkSetting networkSetting)
        {
            logger = loggerFactory.CreateLogger<TelloSettings>();
            Network = networkSetting;
        }


        [JsonIgnore]
        public string ConfigurationFile { get { return Path.Combine(workingDirectory, CONFIG_FILE_NAME); } }

        public void Load(string dir)
        {
            workingDirectory = dir;
            if (!LoadSettings(ConfigurationFile))
                Init();
        }

        public bool Save()
        {
            using (logger.BeginScope(nameof(Save)))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(workingDirectory))
                    {
                        throw new InvalidOperationException("Can't save configuration without load first.");
                    }
                    var config = JsonConvert.SerializeObject(this);
                    File.WriteAllText(ConfigurationFile, config, Encoding.UTF8);
                    logger.LogDebug("save settings to file : {0}", ConfigurationFile);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Save setting file failed.");
                    return false;
                }
            }
        }



        #region Properties
        [JsonProperty]
        public INetworkSetting Network { get; set; }


        #endregion


        private bool LoadSettings(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    throw new ArgumentException("Given file is not exists.", "file");
                }
                logger.LogDebug("Load settings file : {0}", file);
                var config = File.ReadAllText(file, Encoding.UTF8);
                var settings = JsonConvert.DeserializeObject<TelloSettings>(config);
                LoadSettings(settings);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Load setting file failed.");
                return false;
            }
        }

        private void LoadSettings(TelloSettings settings)
        {
            Network.Load(settings.Network);
            //TODO other settings

        }

        private void Init()
        {
            Network.Init();


            logger.LogDebug("Reset Settings.");
        }

    }

    [JsonObject]
    public class NetworkSetting : INetworkSetting
    {
        [JsonProperty]
        public string SSID { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public IPEndPoint IPv4 { get; set; }

        [JsonProperty]
        public bool AutoReconnect { get; set; }

        public NetworkSetting()
        {
            SSID = "TELLO";
            Password = "";
            IPv4 = new IPEndPoint(IPAddress.Any, 0);
            AutoReconnect = false;
        }

        public void Load(INetworkSetting network)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }


}
