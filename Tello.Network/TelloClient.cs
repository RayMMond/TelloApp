using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tello.Core.Network
{
    public class TelloClient : ITelloClient
    {
        private readonly ILogger logger;
        private readonly MultithreadEventLoopGroup group;
        private readonly INetworkSetting setting;


        IChannel clientChannel;


        public TelloClient(ILoggerFactory loggerFactory,
            INetworkSetting setting)
        {

            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            logger = loggerFactory?.CreateLogger<TelloClient>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            if (InternalLoggerFactory.DefaultFactory.GetType() != loggerFactory.GetType())
                InternalLoggerFactory.DefaultFactory = loggerFactory;


            group = new MultithreadEventLoopGroup();
        }



        public async Task<bool> BindPortAsync(SimpleChannelInboundHandler<DatagramPacket> telloClientHandler, string name)
        {
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<SocketDatagramChannel>()
                    .Option(ChannelOption.SoBroadcast, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        channel.Pipeline.AddLast(name, telloClientHandler);
                    }));

                clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task SendAsync(string msg)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                await SendRawAsync(bytes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Send message ({0}) failed. {1}", msg, ex.Message);
            }
        }


        public async Task SendRawAsync(byte[] rawData)
        {
            try
            {
                await clientChannel.SendRawAsync(rawData, setting.IPv4);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Send message ({0}) failed. {1}", string.Join(",", rawData), ex.Message);
            }
        }

        public async Task CloseAsync()
        {
            await clientChannel.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }


    }

}
