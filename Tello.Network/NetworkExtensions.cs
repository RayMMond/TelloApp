
namespace DotNetty.Transport.Channels
{
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels.Sockets;
    using System.Net;
    using System.Threading.Tasks;

    static class NetworkExtensions
    {
        public static void SendRaw(this IChannel channel, byte[] raw, IPEndPoint ip)
        {
            IByteBuffer buffer = Unpooled.WrappedBuffer(raw);
            var task = channel.WriteAndFlushAsync(
                  new DatagramPacket(
                      buffer,
                      ip));

            task.Wait();
        }

        public static async Task SendRawAsync(this IChannel channel, byte[] raw, IPEndPoint ip)
        {
            IByteBuffer buffer = Unpooled.WrappedBuffer(raw);
            await channel.WriteAndFlushAsync(
                  new DatagramPacket(
                      buffer,
                      ip));
        }

    }
}
