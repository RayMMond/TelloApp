using System;
using System.Threading.Tasks;

namespace Tello.Core
{
    public interface IConnectionController
    {
        ITelloClient Client { get; }

        IWiFiManager WiFi { get; }

        bool Connected { get; }

        int iFrameRate { get; set; }


        event EventHandler<VideoStreamReceivedEventArgs> VideoStreamReceived;

        Task<bool> ConnectAsync();

        Task DisconnectAsync();
    }

    public class VideoStreamReceivedEventArgs : EventArgs
    {
        public readonly byte[] Buffer;

        public VideoStreamReceivedEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}