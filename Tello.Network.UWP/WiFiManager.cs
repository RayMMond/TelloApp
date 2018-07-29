namespace Tello.Core.Network.UWP
{
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using Tello.Core;
    using Windows.Devices.WiFi;
    using System.Collections.Generic;
    using Windows.Security.Credentials;

    public class WiFiManager : IWiFiManager
    {
        WiFiAvailableNetwork availableNetwork;

        public WiFiManager(INetworkSetting networkSetting)
        {
            Setting = networkSetting ?? throw new ArgumentNullException(nameof(networkSetting));
            IsConnected = false;


        }



        public bool IsConnected { get; private set; }

        public byte SignalBars
        {
            get
            {
                if (availableNetwork != null)
                    return availableNetwork.SignalBars;
                else
                    return 0;
            }
        }

        public INetworkSetting Setting { get; }

        public string ScanTimestamp { get; private set; } = "";


        public ObservableCollection<WiFiAvailableNetwork> Networks { get; private set; } = new ObservableCollection<WiFiAvailableNetwork>();

        public WiFiAdapter WiFiAdapter { get; private set; }


        public async Task<bool> ConnectAsync()
        {
            if (string.IsNullOrWhiteSpace(Setting.SSID))
                throw new InvalidOperationException("SSID can not be null!");

            await ScanAsync();

            if (Networks.Count > 0)
            {
                var network = Networks.FirstOrDefault(n => n.Ssid == Setting.SSID);
                if (network != null)
                {
                    availableNetwork = network;
                    WiFiConnectionResult result;
                    WiFiReconnectionKind kind = Setting.AutoReconnect ? WiFiReconnectionKind.Automatic : WiFiReconnectionKind.Manual;
                    if (string.IsNullOrWhiteSpace(Setting.Password))
                    {
                        result = await WiFiAdapter.ConnectAsync(network, kind);
                    }
                    else
                    {
                        var password = new PasswordCredential("Tello", "admin", Setting.Password);
                        result = await WiFiAdapter.ConnectAsync(network, kind, password);
                    }

                    if (result.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        IsConnected = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Can not find SSID:{Setting.SSID}.");
                }
            }
            else
            {
                throw new InvalidOperationException("Can not find any WiFi signal.");
            }
        }

        public void Disconnect()
        {
            WiFiAdapter.Disconnect();
        }


        public async Task InitializeWiFiAdapterAsync()
        {

            var list = await WiFiAdapter.FindAllAdaptersAsync();


            Guid adapterId = default(Guid);
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            if (!string.IsNullOrWhiteSpace(Setting.WiFiAdapterName))
            {
                var nic = networkInterfaces.FirstOrDefault(i => i.Name == Setting.WiFiAdapterName);
                if (nic != null)
                {
                    adapterId = Guid.Parse(nic.Id);
                }
            }

            if (adapterId == default(Guid))
            {
                if (list.Count > 0)
                {
                    WiFiAdapter = list.First();
                }
                else
                {
                    throw new InvalidOperationException("No WiFi adapter was found.");
                }
            }
            else
            {
                var adapter = list.FirstOrDefault(a => a.NetworkAdapter.NetworkAdapterId == adapterId);
                if (adapter != null)
                {
                    WiFiAdapter = adapter;
                }
                else
                {
                    throw new InvalidOperationException($"Can not find WiFi adapter, name:{Setting.WiFiAdapterName}.");
                }
            }
        }


        public async Task ScanAsync()
        {
            if (WiFiAdapter != null)
            {
                await WiFiAdapter.ScanAsync();

                ScanTimestamp = WiFiAdapter.NetworkReport.Timestamp.ToString("MM-dd hh:mm:ss");

                Networks.Clear();
                foreach (var n in WiFiAdapter.NetworkReport.AvailableNetworks)
                {
                    Networks.Add(n);
                }
            }
            else
                throw new InvalidOperationException("Cannot scan WiFi signal without WiFi adapter.");
        }

    }
}
