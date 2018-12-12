﻿using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.service.npc;
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

        private GameState gameState;

        private PlayerService playerService;
        private WeaponService weaponService = new WeaponService();
        private NpcService npcService;

        private MiscManager miscManager = new MiscManager();
        private PackageHandler packageHandler;

        public GameManager(GameState gameState)
        {
            this.gameState = gameState;

            playerService = new PlayerService(gameState);
            npcService = new NpcService(gameState);
            packageHandler = new PackageHandler(gameState);
        }

        public void DoGameLogic(float deltaTime)
        {
            npcService.Update(deltaTime);

            CalculateMovements(deltaTime);
            CalculatePhysics(deltaTime);

            miscManager.CheckForInactivity(gameState);
        }

        public void CalculateMovements(float deltaTime)
        {
            // Check to spwan zombies // hold a timestamp on each player and if 10 seconds has passed spawn a zombie close to them
            // If zombie doesnt have a target close enough tell client's that it's wandering around randomly
        }

        public void CalculatePhysics(float deltaTime)
        {
            // Am I ever gonna do this? YES!
        }

        public int AddNewPlayer(IPEndPoint ep)
        {
            foreach (var player in gameState.players)
            {
                if (ep.Address.ToString() == player.Ep.Address.ToString() && ep.Port == player.Ep.Port)
                {
                    // Player already got an ID
                    return player.PlayerId;
                }
            }
            int newPlayerId = gameState.GetNextUID();
            Player p = new Player(newPlayerId, 0, 3, 0, 0, 0, 0, ep);
            weaponService.CreateWeaponsForNewPlayer(p);
            gameState.players.Add(p);
            return newPlayerId;
        }

        public string HandlePlayerStateInformation(String message)
        {
            string returnString = "-1";

            string[] packageArray = packageHandler.GetPlayer(message, out Player player);
            if (player == null) return returnString;

            if (packageArray.Length == 0) { return returnString; }

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
            return returnString;
        }

        public void BroadcastGameState()
        {
            string constructedPlayerPositions = playerService.ConstructAllPlayerPostitions() + npcService.ConstructAllNpcPostitions();
            // Dont broadcast anything if there is nothing to broadcast
            if (constructedPlayerPositions.Length != 0)
            {
                string playerPositions = "002" + constructedPlayerPositions;
                Logger.Log("BroadcastGameState ===> " + playerPositions, ActivityLogEnum.NORMAL);
                var dg = Encoding.ASCII.GetBytes(playerPositions);
                foreach (var player in gameState.players)
                {
                    gameState.udpClient.Send(dg, dg.Length, player.Ep);
                }
            }
        }

        public Player FindPlayerById(int playerId)
        {
            foreach (var player in gameState.players)
            {
                if (playerId == player.PlayerId)
                {
                    return player;
                }
            }
            return null;
        }
    }
}
