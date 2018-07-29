using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Tello.Core
{
    public interface IWiFiManager
    {
        bool IsConnected { get; }
        string ScanTimestamp { get; }
        INetworkSetting Setting { get; }
        byte SignalBars { get; }
        

        Task InitializeWiFiAdapterAsync();
        Task ScanAsync();
        Task<bool> ConnectAsync();
        void Disconnect();
    }
}