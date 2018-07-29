﻿namespace Tello.Core
{
    using System.Threading.Tasks;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    public interface ITelloClient
    {
        Task<bool> BindPortAsync(SimpleChannelInboundHandler<DatagramPacket> telloClientHandler);
        Task CloseAsync();
        Task SendAsync(string msg);
        Task SendRawAsync(byte[] rawData);
    }
}