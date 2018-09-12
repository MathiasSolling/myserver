using myserver.game;
using myserver.game.udp;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace myserver
{
    class HandleData
    {
        GameManager gameManager;

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
            Console.WriteLine("Received message from: " + args.IpAddress.ToString() + ", Port: " + args.Port.ToString() + ", Message: " + message);

            IPEndPoint ep = new IPEndPoint(args.IpAddress, args.Port);
            if (message.StartsWith("000"))
            {
                // New client needs an ID
                int playerId = gameManager.AddNewPlayer(ep);
                // todo if we save stats for next login we can pass saved player coords back here
                Console.WriteLine("New player with id " + playerId + " and EP " + ep.Address + ":" + ep.Port);
                var dg = Encoding.ASCII.GetBytes("000;" + playerId);
                sender.Send(dg, dg.Length, ep);
            }
            else if (message.StartsWith("001"))
            {
                // Receiving client position and actions (package)
                String confirmationString = gameManager.UpdatePlayerState(message);
                if (!confirmationString.Equals("-1"))
                {
                    Console.WriteLine("Answer to package from Client => " + confirmationString + " and EP " + ep.Address + ":" + ep.Port);
                    var dg = Encoding.ASCII.GetBytes(confirmationString);
                    sender.Send(dg, dg.Length, ep);
                }
            }
            else if (message.StartsWith("002"))
            {
                Console.WriteLine("LOOOOL");
            }
        }

    }
}
