using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.service.gametype;
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
using System.Runtime.CompilerServices;
using System.Text;

namespace myserver.game
{
    class GameManager
    {
        private static ActivityLog Logger = new ActivityLog("GameManager");

        private GameState gameState;

        private PlayerService playerService;
        private WeaponService weaponService;
        private NpcService npcService;

        private MiscManager miscManager;
        private PackageHandler packageHandler;

        private DeathmatchService deathmatchService;

        private readonly object addNewPlayerLock = new object();

        private bool enableNpcs = false;

        public GameManager(GameState gameState)
        {
            this.gameState = gameState;

            playerService = new PlayerService(gameState);
            npcService = new NpcService(gameState);
            packageHandler = new PackageHandler(gameState);
            deathmatchService = new DeathmatchService(gameState);

            weaponService = new WeaponService();
            miscManager = new MiscManager();
        }
        
        public void DoGameLogic(float deltaTime)
        {
            // Respawns dead players
            deathmatchService.ReviveDeadPlayers();

            // Updates position, movement, target and atttack players
            if (enableNpcs)
            {
                npcService.Update(deltaTime);
            }

            // todo
            CalculateMovements(deltaTime);
            CalculatePhysics(deltaTime);

            // todo - first make a ping system to see if player is connected
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
        
        public int AddNewPlayer(IPEndPoint ep, string userName)
        {
            lock (addNewPlayerLock)
            {
                foreach (KeyValuePair<int, Player> entry in gameState.players)
                {
                    if (ep.Address.ToString() == entry.Value.Ep.Address.ToString() && ep.Port == entry.Value.Ep.Port)
                    {
                        // Player already got an ID
                        return entry.Value.playerId;
                    }
                }

                int newPlayerId = gameState.GetNextUID();
                Vector3 randomSpawnPosition = deathmatchService.GetRandomSpawnPosition();
                Player player = new Player(newPlayerId, userName, randomSpawnPosition, 0, 0, 0, ep);
                weaponService.CreateWeaponsForNewPlayer(player);
                gameState.players[newPlayerId] = player;
                return newPlayerId;
            }
        }

        public string HandlePlayerStateInformation(String message)
        {
            string returnString = "-1";

            string[] packageArray = packageHandler.GetPlayer(message, out Player player);
            if (player == null)
            {
                return returnString;
            }
            else if (player.dead)
            {
                return returnString;
            }
            else if (packageArray.Length == 0)
            {
                return returnString;
            }

            bool playerNeedsCorrection = packageHandler.GetPlayerStateInformation(packageArray, player, out Dictionary<PlayerStateActionEnum, float> actions);
            if (!playerNeedsCorrection)
            {
                playerService.UpdatePlayer(player, actions);

                // send packageSeqNum that server has processed
                returnString = "001;" + player.packageSeq.ToString();
            }
            else
            {
                // send correction package to player because player sends packeages with too high seq number
                // todo
                returnString = "e001;2:" + player.positionX + ",3:" + player.positionY + ",4:" + player.positionZ;
            }
            return returnString;
        }

        public void BroadcastGameState()
        {
            string gameStateString = playerService.ConstructAllPlayerPostitions() + npcService.ConstructAllNpcPostitions();
            // Dont broadcast anything if there is nothing to broadcast
            if (gameStateString.Length != 0)
            {
                gameStateString = "002" + gameStateString;
                Logger.Log("BroadcastGameState ===> " + gameStateString, ActivityLogEnum.NORMAL);
                var dg = Encoding.ASCII.GetBytes(gameStateString);
                foreach (KeyValuePair<int, Player> entry in gameState.players)
                {
                    gameState.udpClient.Send(dg, dg.Length, entry.Value.Ep);
                }
            }
        }
    }
}
