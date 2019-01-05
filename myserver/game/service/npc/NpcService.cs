using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.npc.zombie;
using myserver.game.service.npc.zombie;
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
        private ZombieService zombieService;

        public NpcService(GameState gameState)
        {
            this.gameState = gameState;
            zombieService = new ZombieService(gameState);
        }

        public void Update(float deltaTime)
        {
            CheckToSpawnNpc();
            UpdateNpc(deltaTime);
        }

        private void CheckToSpawnNpc()
        {
            return;
            zombieService.CheckToSpawnZombie();
        }

        private void UpdateNpc(float deltaTime)
        {
            zombieService.UpdateZombie(deltaTime);
        }

        public String ConstructAllNpcPostitions()
        {
            string npcPositions = "";

            foreach (KeyValuePair<int, Zombie> entry in gameState.zombies)
            {
                String state = entry.Value.RetrieveNewState();
                if (state != null)
                {
                    npcPositions += state;
                }
            }
            return npcPositions;
        }
    }
}
