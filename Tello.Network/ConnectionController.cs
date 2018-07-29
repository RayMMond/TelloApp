using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tello.Network.Protocol;

namespace Tello.Core.Network
{
    public class ConnectionController : IConnectionController, IDisposable
    {
        private bool disposedValue = false;
        private INetworkSetting setting;
        private TelloClientHandler handler;




        public ConnectionController(INetworkSetting setting,
            ITelloClient telloClient,
            IWiFiManager wifiManager,
            TelloClientHandler handler)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            Client = telloClient ?? throw new ArgumentNullException(nameof(telloClient));
            WiFi = wifiManager ?? throw new ArgumentNullException(nameof(wifiManager));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            handler.Connected += Handler_Connected;
            Connected = false;
        }


        public ITelloClient Client { get; }

        public IWiFiManager WiFi { get; }

        public bool Connected { get; private set; }

        public async Task<bool> ConnectAsync()
        {
            if (!await WiFi.ConnectAsync())
            {
                return false;
            }



            if (!await Client.BindPortAsync(handler))
                return false;

            await Client.SendRawAsync(TelloCommand.CONN_REQ);

            await Task.Run(() =>
            {
                while (!Connected)
                    Thread.Sleep(100);
            });

            return true;
        }


        private void Handler_Connected(object sender, EventArgs e)
        {
            Connected = true;
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    handler.Connected -= Handler_Connected;
                }

                handler = null;

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
