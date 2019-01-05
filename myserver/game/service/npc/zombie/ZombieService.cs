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

namespace myserver.game.service.npc.zombie
{
    class ZombieService
    {
        private static ActivityLog Logger = new ActivityLog("ZombieService");

        private GameState gameState;

        private long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long lastTimeHostileZombieSpawned = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 8000;
        private long hostileZombieSpawnInterval = 10000;

        // Time to go back to find the player position where we want to spawn the zombie
        private long timeToGoBack = 5000;

        public ZombieService(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void CheckToSpawnZombie()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (lastTimeHostileZombieSpawned + hostileZombieSpawnInterval < currentTime)
            {
                lastTimeHostileZombieSpawned = currentTime;

                Random rand = new Random();

                // Set new random spawn interval
                hostileZombieSpawnInterval = rand.Next(5000, 15000);
                
                int tries = 0;
                bool done = false;
                while (!done)
                {
                    tries++;

                    if (gameState.players.Any())
                    {
                        // Find random player to spawn zombie at
                        Player player = gameState.players.ElementAt(rand.Next(0, gameState.players.Count)).Value;

                        if (player.timeOfLoggedIn < currentTime - timeToGoBack)
                        {
                            // Find atleast X second old position of random player
                            var dict = player.psaKeyValueHistory.Where(x => x.Key < currentTime - timeToGoBack && x.Key > currentTime - timeToGoBack - 2000);
                            if (dict.Any())
                            {
                                // Reverse the dictionary and take first value, this is the closest one to the X seconds old position
                                ConcurrentDictionary<int, float> innerDict = dict.First().Value;

                                // Check if this state of the player contains position values
                                if (innerDict.TryGetValue((int)PlayerStateActionEnum.PosX, out float posX) &&
                                    innerDict.TryGetValue((int)PlayerStateActionEnum.PosY, out float posY) &&
                                    innerDict.TryGetValue((int)PlayerStateActionEnum.PosZ, out float posZ))
                                {
                                    Vector3 randomPlayerRandomPosition = new Vector3(posX, posY, posZ);
                                    Vector3 randomPlayerCurrentPosition = new Vector3(player.positionX, player.positionY, player.positionZ);
                                    // Make sure we don't spawn zombie on top of player
                                    if (Vector3.Distance(randomPlayerRandomPosition, randomPlayerCurrentPosition) > 3)
                                    {
                                        // Spawn new zombie
                                        int newZombieId = gameState.GetNextUID();
                                        gameState.zombies[newZombieId] = new Zombie(newZombieId, randomPlayerRandomPosition);
                                        Logger.Log("New Zombie spawned", ActivityLogEnum.NORMAL);

                                        done = true;
                                    }
                                }
                            }
                        }
                    }

                    if (tries >= gameState.players.Count)
                    {
                        done = true;
                    }
                }
            }
        }

        public void UpdateZombie(float deltaTime)
        {
            foreach (KeyValuePair<int, Zombie> entry in gameState.zombies)
            {
                var zombie = entry.Value;
                if (zombie.dead) continue;
                zombie.UpdateNpcTarget(gameState.players);
                zombie.MoveAndRotate(deltaTime);
                zombie.AttackTarget();
            }
        }
    }
}
