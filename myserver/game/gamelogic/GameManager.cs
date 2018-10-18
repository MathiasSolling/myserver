using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.service.weapon;
using myserver.game.udp;
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
    class GameManager
    {
        private static ActivityLog Logger = new ActivityLog("GameManager");

        private int GameId { get; set; }
        public ConcurrentBag<Player> Players { get; set; }

        private PlayerService playerService;
        private WeaponService weaponService = new WeaponService();

        private MiscManager miscManager = new MiscManager();
        private PackageHandler packageHandler;

        private UdpClient udpClient;

        public GameManager(int gameId, UdpClient udpClient)
        {
            this.GameId = gameId;
            this.udpClient = udpClient;

            Players = new ConcurrentBag<Player>();
            playerService = new PlayerService(Players);
            packageHandler = new PackageHandler(Players);
        }

        public void DoGameLogic(double deltaTime)
        {
            CalculateMovements(deltaTime);
            CalculatePhysics();

            miscManager.CheckForInactivity(Players);
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
            foreach (var player in Players)
            {
                if (ep.Address.ToString() == player.Ep.Address.ToString() && ep.Port == player.Ep.Port)
                {
                    // Player already got an ID
                    return player.PlayerId;
                }
            }
            int newPlayerId = Players.Count() + 1;
            Player p = new Player(newPlayerId, 0, 3, 0, 0, 0, 0, ep);
            weaponService.CreateWeaponsForNewPlayer(p);
            Players.Add(p);
            return newPlayerId;
        }

        public string HandlePlayerStateInformation(String message)
        {
            string[] packageArray = packageHandler.GetPlayer(message, out Player player);

            string returnString = "-1";
            if (packageArray.Length == 0)
            {
                return returnString;
            }
            if (player != null)
            {
                bool playerNeedsCorrection = packageHandler.GetPlayerStateInformation(packageArray, player, out Dictionary<int, float> actions);
                if (!playerNeedsCorrection)
                {
                    playerService.UpdatePlayer(player, actions);

                    // send packageSeqNum that server has processed
                    returnString = "001;" + player.PackageSeq.ToString();
                }
                else
                {
                    // send correction package to player because player sends packeages with too high seq number
                    // todo
                    returnString = "e001;2:" + player.PositionX + ",3:" + player.PositionY + ",4:" + player.PositionZ;
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
                Logger.Log("BroadcastGameState ===> " + playerPositions, ActivityLogEnum.NORMAL);
                var dg = Encoding.ASCII.GetBytes(playerPositions);
                foreach (var player in Players)
                {
                    udpClient.Send(dg, dg.Length, player.Ep);
                }
            }
        }

        public Player FindPlayerById(int playerId)
        {
            foreach (var player in Players)
            {
                if (playerId == player.PlayerId)
                {
                    return player;
                }
            }
            return null;
        }

        public void SendUDPMessage(Player player, string message)
        {
            var dg = Encoding.ASCII.GetBytes(message);
            udpClient.Send(dg, dg.Length, player.Ep);
        }
    }
}
