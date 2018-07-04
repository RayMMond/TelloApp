using System.Net;

namespace Tello.Core
{
    public interface INetworkSetting
    {
        bool AutoReconnect { get; set; }
        IPEndPoint IPv4 { get; set; }
        string Password { get; set; }
        string SSID { get; set; }

        void Init();
        void Load(INetworkSetting network);
    }
}