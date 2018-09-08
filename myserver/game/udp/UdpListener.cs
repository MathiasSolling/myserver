﻿using myserver.game.udp;
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
        UdpClient udpClient;
        public const int SIO_UDP_CONNRESET = -1744830452;

        private bool disposed = false;

        IPEndPoint listenEP = new IPEndPoint(IPAddress.Any, 36200);

        public UdpListener()
        {
            udpClient = new UdpClient(listenEP);
            //Don't throw exception if connection to a client has been lost
            udpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);
        }

        public void Listen()
        {
            try
            {
                udpClient.BeginReceive(new AsyncCallback(DetectionCallback), udpClient);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void DetectionCallback(IAsyncResult res)
        {
            try
            {
                var client = (res.AsyncState as UdpClient);
                if (client.Client == null) return;

                var endPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.EndReceive(res, ref endPoint);
                udpClient.BeginReceive(new AsyncCallback(DetectionCallback), client);

                //Process codes
                RaiseDataReceived(new ReceivedDataArgs(endPoint.Address, endPoint.Port, data));
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
