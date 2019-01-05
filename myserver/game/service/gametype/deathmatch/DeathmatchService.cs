using myserver.game.gamelogic;
using myserver.game.service.gametype.deathmatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service.gametype
{
    class DeathmatchService
    {
        private GameState gameState;

        private Random rand = new Random();

        private List<PlayerSpawnArea> playersSpawns = new List<PlayerSpawnArea>();

        private long timeToRespawn = 10000;

        public DeathmatchService(GameState gameState)
        {
            this.gameState = gameState;
            Init();
        }

        private void Init()
        {
            playersSpawns.Add(new PlayerSpawnArea(new Point(-8, 34), new Point(-7.5, 35), 6.1f, rand));
            playersSpawns.Add(new PlayerSpawnArea(new Point(38, 15), new Point(38.5, 14.5), 6.1f, rand));
        }

        public void ReviveDeadPlayers()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (KeyValuePair<int, Player> entry in gameState.players)
            {
                var player = entry.Value;
                if (player.dead && player.timeOfDeath + timeToRespawn < currentTime)
                {
                    PlayerSpawnArea psa = playersSpawns.ElementAt(rand.Next(0, playersSpawns.Count));
                    player.Respawn(psa.GetRandomPosInArea());
                }
            }
        }

    }
}
