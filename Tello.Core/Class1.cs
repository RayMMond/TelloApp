using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
namespace Tello.Core
{
    class Program
    {
        static int port = 1234;
        static async Task RunClientAsync()
        {
            //ExampleHelper.SetConsoleLogger();
            
            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<SocketDatagramChannel>()
                    .Option(ChannelOption.SoBroadcast, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        channel.Pipeline.AddLast("Quote", new QuoteOfTheMomentClientHandler());
                    }));

                IChannel clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);

                Console.WriteLine("Sending broadcast QOTM");

                // Broadcast the QOTM request to port.
                byte[] bytes = Encoding.UTF8.GetBytes("QOTM?");
                IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
                await clientChannel.WriteAndFlushAsync(
                    new DatagramPacket(
                        buffer,
                        new IPEndPoint(IPAddress.Broadcast, port)));

                Console.WriteLine("Waiting for response.");

                await Task.Delay(5000);
                Console.WriteLine("Waiting for response time 5000 completed. Closing client channel.");

                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        static void Main() => RunClientAsync().Wait();
    }
}
