using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Data;
using System.Globalization;
using Tello.Core;

namespace TelloApp
{
    public class WiFiAvailableNetworkInfo : IWiFiAvailableNetworkInfo
    {
        WiFiAvailableNetwork __network;

        public WiFiAvailableNetworkInfo(WiFiAvailableNetwork n)
        {
            __network = n;
        }

        public TimeSpan BeaconInterval => __network.BeaconInterval;

        public string Bssid => __network.Bssid;

        public int ChannelCenterFrequencyInKilohertz => __network.ChannelCenterFrequencyInKilohertz;

        public bool IsWiFiDirect => __network.IsWiFiDirect;

        public WiFiNetworkKind NetworkKind => __network.NetworkKind;

        public double NetworkRssiInDecibelMilliwatts => __network.NetworkRssiInDecibelMilliwatts;

        public WiFiPhyKind PhyKind => __network.PhyKind;

        public NetworkSecuritySettings SecuritySettings => __network.SecuritySettings;

        public byte SignalBars => __network.SignalBars;

        public string Ssid => __network.Ssid;


        public override string ToString()
        {
            return $"SSID:{Ssid} SignalBars:{SignalBars}";
        }
    }

    internal interface IWiFiAvailableNetworkInfo
    {
        TimeSpan BeaconInterval { get; }
        string Bssid { get; }
        int ChannelCenterFrequencyInKilohertz { get; }
        bool IsWiFiDirect { get; }
        WiFiNetworkKind NetworkKind { get; }
        double NetworkRssiInDecibelMilliwatts { get; }
        WiFiPhyKind PhyKind { get; }
        NetworkSecuritySettings SecuritySettings { get; }
        byte SignalBars { get; }
        string Ssid { get; }
    }


    public class NetworkInfoToStringConverter : IValueConverter
    {

        #region IValueConverter Members


        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // The value parameter is the data from the source object.
            WiFiAvailableNetworkInfo info = (WiFiAvailableNetworkInfo)value;

            QuoteOfTheMomentClientHandler s = new QuoteOfTheMomentClientHandler();

            return $"ssid:{info.Ssid} NetworkRssi:{info.NetworkRssiInDecibelMilliwatts} ";
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
