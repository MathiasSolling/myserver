using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.udp
{
    public class ReceivedDataArgs
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public byte[] ReceivedBytes;

        public ReceivedDataArgs(IPAddress ip, int port, byte[] data)
        {
            this.IpAddress = ip;
            this.Port = port;
            this.ReceivedBytes = data;
        }
    }
}
