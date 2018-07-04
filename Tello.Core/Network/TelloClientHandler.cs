using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Core.Network
{
    public class TelloClientHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        private ILogger logger;

        public TelloClientHandler(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            logger = loggerFactory.CreateLogger<TelloClientHandler>();
        }


        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            logger.LogDebug("Received => {0}", packet);
            if (!packet.Content.IsReadable())
            {
                return;
            }


            string message = packet.Content.ToString(Encoding.UTF8);
            logger.LogDebug("Received Message => {0}", message);


            //ctx.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            logger.LogError(exception, exception.Message);
        }
    }
}
