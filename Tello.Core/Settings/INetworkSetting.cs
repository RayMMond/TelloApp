namespace Tello.Core
{
    using System.Net;

    public interface INetworkSetting
    {
        bool AutoReconnect { get; set; }
        IPEndPoint IPv4 { get; set; }
        int VideoStreamPort { get; set; }
        string Password { get; set; }
        string SSID { get; set; }
        string WiFiAdapterName { get; set; }

        void Load(INetworkSetting network);
    }
}