using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.npc.zombie;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.npc
{
    class NpcService
    {
        private static ActivityLog Logger = new ActivityLog("NpcService");
        private GameState gameState;

        private long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long lastTimeHostileNpcSpawned = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 12000;
        private long hostileNpcSpawnInterval = 20000;

        public NpcService(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void Update(float deltaTime)
        {
            CheckToSpawnNpc();
            UpdateNpc(deltaTime);
        }

        private void CheckToSpawnNpc()
        {
            // Max 50 zombies with a maximum spawn-rate of 2 seconds
            currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (lastTimeHostileNpcSpawned + hostileNpcSpawnInterval < currentTime && gameState.zombies.Count < 50)
            {
                lastTimeHostileNpcSpawned = currentTime;

                // Increase spawn interval
                if (hostileNpcSpawnInterval > 2000)
                {
                    hostileNpcSpawnInterval -= 500;
                }

                gameState.zombies.Add(new Zombie(gameState.GetNextUID()));
                Logger.Log("New Zombie spawned", ActivityLogEnum.NORMAL);
            }
        }

        private void UpdateNpc(float deltaTime)
        {
            
            foreach (var zombie in gameState.zombies)
            {
                if (zombie.dead) return;
                zombie.UpdateNpcTarget(gameState.players);
                zombie.MoveAndRotate(deltaTime);
                zombie.AttackTarget();
            }
        }

        

        public String ConstructAllNpcPostitions()
        {
            string npcPositions = "";

            foreach (var zombie in gameState.zombies)
            {
                String state = zombie.RetrieveNewState();
                if (state != null)
                {
                    npcPositions += state;
                }
            }
            return npcPositions;
        }
    }
}
