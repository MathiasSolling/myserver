using myserver.game.udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myserver
{
    public class UdpListener : IDisposable
    {
        //UdpClient udpClient;
        //public const int SIO_UDP_CONNRESET = -1744830452;

        private bool disposed = false;

        private Socket s;
        public byte[] ReceiveBuffer = new byte[2048];

        public UdpListener()
        {

            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.PacketInformation, true);
            s.Bind(new IPEndPoint(IPAddress.Any, 36200));
            // udpClient = new UdpClient(36200);
            // Don't throw exception if connection to a client has been lost
            // udpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
        }

        public void Listen()
        {
            try
            {
                //udpClient.BeginReceive(new AsyncCallback(Recv), null);
                EndPoint remoteEndPoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                s.BeginReceiveMessageFrom(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, ref remoteEndPoint, Recv, s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void Recv(IAsyncResult res)
        {
            //IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                //byte[] data = udpClient.EndReceive(res, ref clientEndPoint);
                //udpClient.BeginReceive(new AsyncCallback(Recv), null);

                Socket receiveSocket = (Socket)res.AsyncState;

                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint ep = (EndPoint) clientEndPoint;
                IPPacketInformation packetInfo;
                SocketFlags flags = SocketFlags.None;
                int udpMessageLength = receiveSocket.EndReceiveMessageFrom(res, ref flags, ref ep, out packetInfo);
                byte[] udpMessage = new byte[udpMessageLength];
                Array.Copy(ReceiveBuffer, udpMessage, udpMessageLength);

                Console.WriteLine(
                    "{0} bytes received from {1} to {2}",
                    ReceiveBuffer,
                    ep,
                    packetInfo.Address
                );
                
                s.BeginReceiveMessageFrom(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, ref ep, Recv, s);

                //Process codes
                RaiseDataReceived(new ReceivedDataArgs(((IPEndPoint)ep).Address, ((IPEndPoint)ep).Port, udpMessage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public delegate void DataReceived(ReceivedDataArgs args);

        public event DataReceived DataReceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs args)
        {
            DataReceivedEvent?.Invoke(args);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //udpClient.Dispose();
                }
                disposed = true;
            }
        }

        ~UdpListener()
        {
            Dispose(false);
        }
    }

}
