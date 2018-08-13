using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Network.Protocol
{
    public static class TelloCommand
    {
        public static readonly byte[] CONN_REQ = Encoding.ASCII.GetBytes("conn_req:\x96\x17");

        public static readonly byte[] IFRAME = new byte[] { 0xcc, 0x58, 0x00, 0x7c, 0x60, 0x25, 0x00, 0x00, 0x00, 0x6c, 0x95 };

    }

}
