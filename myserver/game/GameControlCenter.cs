using System;
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
        public List<Player> Players { get; set; }

        private PlayerService playerService = new PlayerService();
        
        public GameControlCenter(int gameId)
        {
            this.GameId = gameId;
            Players = new List<Player>();
        }

        public void DoGameLogic(double deltaTime)
        {
            CalculateMovements(deltaTime);
            CalculatePhysics();
        }

        public void CalculateMovements(double deltaTime)
        {
            // Am I ever gonna do this?
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
            // So now we should just have a list of packages
            string returnString = "-1";
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
                        returnString = "5001;2:" + player.PositionX + ",3:" + player.PositionY + ",4:" + player.PositionZ;
                    }
                }
            }
            return returnString;
        }

        public string ConstructAllPlayerPostitions()
        {
            string playerPostitions = "";
            foreach (var player in Players)
            {
                playerPostitions += playerService.RetrieveNewPlayerState(player);
            }
            return playerPostitions;
        }

        public void BroadcastGameState()
        {
            UdpClient udpClient = new UdpClient();
            string constructedPlayerPositions = ConstructAllPlayerPostitions();
            // Dont broadcast anything if there is nothing to broadcast
            if (constructedPlayerPositions.Length != 0)
            {
                string playerPositions = "002" + constructedPlayerPositions;
                Console.WriteLine("BroadcastGameState => " + playerPositions);
                var dg = Encoding.ASCII.GetBytes(playerPositions);
                foreach (var player in Players)
                {
                    udpClient.Send(dg, dg.Length, player.Ep);
                }
            }
        }

        private int BoolToInt(bool isTrue)
        {
            if (isTrue)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
