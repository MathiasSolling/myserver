using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace myserver.game
{
    class GameControlCenter
    {
        private int GameId { get; set; }
        public ConcurrentBag<Player> Players { get; set; }

        private PlayerService playerService = new PlayerService();

        private UdpClient udpClient = new UdpClient();
        
        public GameControlCenter(int gameId)
        {
            this.GameId = gameId;
            Players = new ConcurrentBag<Player>();
        }

        public void DoGameLogic(double deltaTime)
        {
            CalculateMovements(deltaTime);
            CalculatePhysics();
        }

        public void CalculateMovements(double deltaTime)
        {
            // Check to spwan zombies // hold a timestamp on each player and if 10 seconds has passed spawn a zombie close to them
            // If zombie doesnt have a target close enough tell client's that it's wandering around randomly
        }

        public void CalculatePhysics()
        {
            // Am I ever gonna do this?
        }

        public int AddNewPlayer(IPEndPoint ep)
        {
            int newPlayerId = Players.Count() + 1;
            Player p = new Player(newPlayerId, 0, 3, 0, 0, 0, 0, ep);
            Players.Add(p);
            return newPlayerId;
        }

        public string UpdatePlayerState(String message)
        {
            // messageSplit[0] is the package type 001
            string[] messageSplit = message.Split(';');
            // messageSplit[1] is the playerId
            int pId = Int32.Parse(messageSplit[1]);
            // Skipping first which is message type (001 player state) and second which is pId
            string[] packageArray = messageSplit.Skip(2).ToArray();

            string returnString = "-1";
            if (packageArray.Length == 0)
            {
                Console.WriteLine("Missing actions in UpdatePlayerState!");
                return returnString;
            }
            // So now we should just have a list of packages
            foreach (var player in Players)
            {
                if (player.PlayerId == pId)
                {
                    bool playerNeedsCorrection = playerService.UpdatePlayerState(player, packageArray);
                    if (!playerNeedsCorrection)
                    {
                        // send packageSeqNum that server has processed
                        returnString = "001;" + player.PackageSeq.ToString();
                    } 
                    else
                    {
                        // send correction package to player
                        // todo
                        returnString = "e001;2:" + player.PositionX + ",3:" + player.PositionY + ",4:" + player.PositionZ;
                    }
                }
            }
            return returnString;
        }

        public string ConstructAllPlayerPostitions()
        {
            string playerPositions = "";
            foreach (var player in Players)
            {
                playerPositions += playerService.RetrieveNewPlayerState(player);
            }
            // Below is for testing purpose when only 1 player on server
            // playerPositions += ";1,8:1";
            return playerPositions;
        }

        public void BroadcastGameState()
        {
            string constructedPlayerPositions = ConstructAllPlayerPostitions();
            // Dont broadcast anything if there is nothing to broadcast
            if (constructedPlayerPositions.Length != 0)
            {
                string playerPositions = "002" + constructedPlayerPositions;
                Console.WriteLine("BroadcastGameState ===> " + playerPositions);
                var dg = Encoding.ASCII.GetBytes(playerPositions);
                foreach (var player in Players)
                {
                    udpClient.Send(dg, dg.Length, player.Ep);
                }
            }
        }
    }
}
