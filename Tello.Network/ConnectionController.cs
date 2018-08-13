using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        private ILogger logger;
        private TelloClientHandler handler;
        private CancellationTokenSource tokenSource;

        public ConnectionController(INetworkSetting setting,
            ITelloClient telloClient,
            IWiFiManager wifiManager,
            ILoggerFactory loggerFactory,
            TelloClientHandler handler)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            Client = telloClient ?? throw new ArgumentNullException(nameof(telloClient));
            WiFi = wifiManager ?? throw new ArgumentNullException(nameof(wifiManager));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            logger = loggerFactory?.CreateLogger<ConnectionController>() ?? throw new ArgumentNullException(nameof(logger));
            handler.Connected += Handler_Connected;
            Connected = false;
            tokenSource = new CancellationTokenSource();
        }


        public event EventHandler<VideoStreamReceivedEventArgs> VideoStreamReceived;


        public ITelloClient Client { get; }

        public IWiFiManager WiFi { get; }

        public bool Connected { get; private set; }

        public int iFrameRate { get; set; } = 5;




        public async Task<bool> ConnectAsync()
        {
            try
            {
                if (!await WiFi.ConnectAsync())
                {
                    return false;
                }


                StartVideoListener();

                if (!await Client.BindPortAsync(handler, "Tello"))
                    return false;

                await Client.SendRawAsync(TelloCommand.CONN_REQ);

                await Task.Run(() =>
                {
                    while (!Connected)
                        Thread.Sleep(100);
                });

                return true;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                await Client.CloseAsync();
                tokenSource.Cancel();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }

            //client.Client.Close();
            Connected = false;

            //if (connectionState != ConnectionState.Disconnected)
            //{
            //    //if changed to disconnect send event
            //    onConnection(ConnectionState.Disconnected);
            //}

            //connectionState = ConnectionState.Disconnected;

        }



        //public static void sendControllerUpdate()
        //{
        //    //if (!connected)
        //    //    return;

        //    //var boost = 0.0f;
        //    //if (controllerState.speed > 0)
        //    //    boost = 1.0f;

        //    ////var limit = 1.0f;//Slow down while testing.
        //    ////rx = rx * limit;
        //    ////ry = ry * limit;

        //    ////Console.WriteLine(controllerState.rx + " " + controllerState.ry + " " + controllerState.lx + " " + controllerState.ly + " SP:"+boost);
        //    //var packet = createJoyPacket(controllerState.rx, controllerState.ry, controllerState.lx, controllerState.ly, boost);
        //    //try
        //    //{
        //    //    client.Send(packet);
        //    //}
        //    //catch (Exception ex)
        //    //{

        //    //}
        //}


        private async void Handler_Connected(object sender, EventArgs e)
        {
            Connected = true;
            StartHeartbeat();
            await RequestIframe();
        }

        private void StartHeartbeat()
        {
            Task.Run(async () =>
            {
                int tick = 0;
                while (true)
                {
                    try
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                            break;
                        //sendControllerUpdate();

                        tick++;
                        if ((tick % iFrameRate) == 0)
                            await RequestIframe();

                        Thread.Sleep(50);//Often enough?
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Heatbeat error:" + ex.Message, ex);
                        await DisconnectAsync();
                        break;
                    }
                }
            }, tokenSource.Token);

        }

        private void StartVideoListener()
        {
            Task.Run(async () =>
            {
                var started = false;
                using (UdpClient listener = new UdpClient(setting.VideoStreamPort))
                {
                    while (true)
                    {
                        try
                        {
                            if (tokenSource.Token.IsCancellationRequested)//handle canceling thread.
                                break;

                            var receiveResult = await listener.ReceiveAsync();
                            var buffer = receiveResult.Buffer;
                            if (buffer[2] == 0 && buffer[3] == 0 && buffer[4] == 0 && buffer[5] == 1)//Wait for first NAL
                            {
                                var nal = (buffer[6] & 0x1f);
                                //if (nal != 0x01 && nal!=0x07 && nal != 0x08 && nal != 0x05)
                                //    Console.WriteLine("NAL type:" +nal);
                                started = true;
                            }

                            if (started)
                            {
                                VideoStreamReceived?.Invoke(this, new VideoStreamReceivedEventArgs(buffer));
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex.Message, ex);
                            break;
                        }
                    }

                }

            }, tokenSource.Token);
        }

        private async Task RequestIframe() => await Client.SendRawAsync(TelloCommand.IFRAME);




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
                VideoStreamReceived = null;

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
