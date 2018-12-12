using myserver.game;
using myserver.game.udp;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using myserver.game.activitylog;

namespace myserver
{
    class HandleData
    {
        private static ActivityLog Logger = new ActivityLog("HandleData");

        private GameManager gameManager;

        public HandleData(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void subscribeToEvent(UdpListener udpListener)
        {
            udpListener.DataReceivedEvent += listener_DataReceivedEvent;
        }

        void listener_DataReceivedEvent(UdpClient sender, ReceivedDataArgs args)
        {
            string message = Encoding.ASCII.GetString(args.ReceivedBytes);
            //Console.WriteLine("Received message from: " + args.IpAddress.ToString() + ", Port: " + args.Port.ToString() + ", Message: " + message);

            IPEndPoint ep = new IPEndPoint(args.IpAddress, args.Port);
            if (message.StartsWith("000"))
            {
                // New client needs an ID
                int playerId = gameManager.AddNewPlayer(ep);
                // todo if we save stats for next login we can pass saved player coords back here
                Logger.Log("New player with id " + playerId + " and EP " + ep.Address + ":" + ep.Port, ActivityLogEnum.NORMAL);
                var dg = Encoding.ASCII.GetBytes("000;" + playerId);
                sender.Send(dg, dg.Length, ep);
            }
            else if (message.StartsWith("001"))
            {
                // Receiving client position and actions (package)
                String confirmationString = gameManager.HandlePlayerStateInformation(message);
                if (!confirmationString.Equals("-1"))
                {
                    // Logger.Log("Answer to package from Client => " + confirmationString + " and EP " + ep.Address + ":" + ep.Port, ActivityLogEnum.NORMAL);
                    var dg = Encoding.ASCII.GetBytes(confirmationString);
                    sender.Send(dg, dg.Length, ep);
                }
            }
            else if (message.StartsWith("002"))
            {
                Logger.Log("LOOOOL", ActivityLogEnum.WARNING);
            }
        }

    }
}
