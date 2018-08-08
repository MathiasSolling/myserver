using myserver.game.udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace myserver
{
    public class UdpListener : IDisposable
    {
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        UdpClient udpClient;
        public const int SIO_UDP_CONNRESET = -1744830452;

        private bool disposed = false;

        public UdpListener(IPEndPoint endpoint)
        {
            serverEndPoint = endpoint;
            udpClient = new UdpClient(serverEndPoint);
            // Don't throw exception if connection to a client has been lost
            udpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
        }

        public void Listen()
        {
            try
            {
                Console.WriteLine("BeginReceive on " + serverEndPoint.Address);
                udpClient.BeginReceive(new AsyncCallback(Recv), null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void Recv(IAsyncResult res)
        {
            byte[] data = udpClient.EndReceive(res, ref clientEndPoint);
            udpClient.BeginReceive(new AsyncCallback(Recv), null);

            //Process codes
            RaiseDataReceived(new ReceivedDataArgs(clientEndPoint.Address, clientEndPoint.Port, data));
        }

        public delegate void DataReceived(UdpClient sender, ReceivedDataArgs args);

        public event DataReceived DataReceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs args)
        {
            if (DataReceivedEvent != null)
            {
                DataReceivedEvent(udpClient, args);
            }
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
                    udpClient.Dispose();
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
