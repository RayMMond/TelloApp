using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tello.Network.Protocol;

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

        public event EventHandler Connected;

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            logger.LogDebug("Received => {0}", packet);
            if (!packet.Content.IsReadable())
            {
                return;
            }


            try
            {
                var received = packet.Content;


                if (received.ToString(Encoding.ASCII).StartsWith("conn_ack"))
                {
                    Connected?.Invoke(this, EventArgs.Empty);

                    // requestIframe();
                    //for(int i=74;i<80;i++)
                    //queryUnk(i);
                    //Console.WriteLine("Tello connected!");
                    return;
                }


                //received
                var cmdId = received.GetShortLE(5);

                // int cmdId = (received.GetInt(5) | ((int)received.bytes[6] << 8));

                if (cmdId >= 74 && cmdId < 80)
                {
                    //Console.WriteLine("XXXXXXXXCMD:" + cmdId);
                }
                //if (cmdId == 86)//state command
                //{
                //    //update
                //    state.set(received.bytes.Skip(9).ToArray());

                //}
                //if (cmdId == 4176)//log header
                //{
                //    //just ack.
                //    var id = BitConverter.ToUInt16(received.bytes, 9);
                //    sendAckLog((short)cmdId, id);
                //    //Console.WriteLine(id);
                //}
                //if (cmdId == 4177)//log data
                //{
                //    state.parseLog(received.bytes.Skip(10).ToArray());
                //}
                //if (cmdId == 4178)//log config
                //{
                //    //todo. this doesnt seem to be working.

                //    //var id = BitConverter.ToUInt16(received.bytes, 9);
                //    //var n2 = BitConverter.ToInt32(received.bytes, 11);
                //    //sendAckLogConfig((short)cmdId, id,n2);

                //    //var dataStr = BitConverter.ToString(received.bytes.Skip(14).Take(10).ToArray()).Replace("-", " ")/*+"  "+pos*/;


                //    //Console.WriteLine(dataStr);
                //}
                //if (cmdId == 4185)//att angle response
                //{
                //    var array = received.bytes.Skip(10).Take(4).ToArray();
                //    float f = BitConverter.ToSingle(array, 0);
                //    Console.WriteLine(f);
                //}
                //if (cmdId == 4182)//max hei response
                //{
                //    //var array = received.bytes.Skip(9).Take(4).Reverse().ToArray();
                //    //float f = BitConverter.ToSingle(array, 0);
                //    //Console.WriteLine(f);
                //    if (received.bytes[10] != 10)
                //    {

                //    }
                //}
                //if (cmdId == 26)//wifi str command
                //{
                //    wifiStrength = received.bytes[9];
                //    if (received.bytes[10] != 0)//Disturb?
                //    {
                //    }
                //}
                //if (cmdId == 53)//light str command
                //{
                //}
                //if (cmdId == 98)//start jpeg.
                //{
                //    picFilePath = picPath + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".jpg";

                //    var start = 9;
                //    var ftype = received.bytes[start];
                //    start += 1;
                //    picBytesExpected = BitConverter.ToUInt32(received.bytes, start);
                //    if (picBytesExpected > picbuffer.Length)
                //    {
                //        Console.WriteLine("WARNING:Picture Too Big! " + picBytesExpected);
                //        picbuffer = new byte[picBytesExpected];
                //    }
                //    picBytesRecived = 0;
                //    picChunkState = new bool[(picBytesExpected / 1024) + 1]; //calc based on size. 
                //    picPieceState = new bool[(picChunkState.Length / 8) + 1];
                //    picExtraPackets = 0;//for debugging.
                //    picDownloading = true;

                //    sendAckFileSize();
                //}
                //if (cmdId == 99)//jpeg
                //{
                //    //var dataStr = BitConverter.ToString(received.bytes.Skip(0).Take(30).ToArray()).Replace("-", " ");

                //    var start = 9;
                //    var fileNum = BitConverter.ToUInt16(received.bytes, start);
                //    start += 2;
                //    var pieceNum = BitConverter.ToUInt32(received.bytes, start);
                //    start += 4;
                //    var seqNum = BitConverter.ToUInt32(received.bytes, start);
                //    start += 4;
                //    var size = BitConverter.ToUInt16(received.bytes, start);
                //    start += 2;

                //    maxPieceNum = Math.Max((int)pieceNum, maxPieceNum);
                //    if (!picChunkState[seqNum])
                //    {
                //        Array.Copy(received.bytes, start, picbuffer, seqNum * 1024, size);
                //        picBytesRecived += size;
                //        picChunkState[seqNum] = true;

                //        for (int p = 0; p < picChunkState.Length / 8; p++)
                //        {
                //            var done = true;
                //            for (int s = 0; s < 8; s++)
                //            {
                //                if (!picChunkState[(p * 8) + s])
                //                {
                //                    done = false;
                //                    break;
                //                }
                //            }
                //            if (done && !picPieceState[p])
                //            {
                //                picPieceState[p] = true;
                //                sendAckFilePiece(0, fileNum, (UInt32)p);
                //                //Console.WriteLine("\nACK PN:" + p + " " + seqNum);
                //            }
                //        }
                //        if (picFilePath != null && picBytesRecived >= picBytesExpected)
                //        {
                //            picDownloading = false;

                //            sendAckFilePiece(1, 0, (UInt32)maxPieceNum);//todo. Double check this. finalize

                //            sendAckFileDone((int)picBytesExpected);

                //            //HACK.
                //            //Send file done cmdId to the update listener so it knows the picture is done. 
                //            //hack.
                //            onUpdate(100);
                //            //hack.
                //            //This is a hack because it is faking a message. And not a very good fake.
                //            //HACK.

                //            Console.WriteLine("\nDONE PN:" + pieceNum + " max: " + maxPieceNum);

                //            //Save raw data minus sequence.
                //            using (var stream = new FileStream(picFilePath, FileMode.Append))
                //            {
                //                stream.Write(picbuffer, 0, (int)picBytesExpected);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        picExtraPackets++;//for debugging.

                //        //if(picBytesRecived >= picBytesExpected)
                //        //    Console.WriteLine("\nEXTRA PN:"+pieceNum+" max "+ maxPieceNum);
                //    }


                //}
                //if (cmdId == 100)
                //{

                //}

                ////send command to listeners. 
                //try
                //{
                //    //fire update event.
                //    onUpdate(cmdId);
                //}
                //catch (Exception ex)
                //{
                //    //Fixed. Update errors do not cause disconnect.
                //    Console.WriteLine("onUpdate error:" + ex.Message);
                //    //break;
                //}


            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                //Console.WriteLine("Receive thread error:" + ex.Message);
                //disconnect();
                //break;
            }
            
        }



        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            logger.LogError(exception, exception.Message);
        }
    }
}
