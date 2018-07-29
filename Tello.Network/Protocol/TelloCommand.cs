using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Network.Protocol
{
    public static class TelloCommand
    {
        public static readonly byte[] CONN_REQ = Encoding.ASCII.GetBytes("conn_req:\x96\x17");



    }

}
