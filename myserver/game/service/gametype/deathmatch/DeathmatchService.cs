using myserver.game.gamelogic;
using myserver.game.service.gametype.deathmatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        private long timeToRespawn = 5000;

        public DeathmatchService(GameState gameState)
        {
            this.gameState = gameState;
            Init();
        }

        private void Init()
        {
            playersSpawns.Add(new PlayerSpawnArea(new Point(236, 300), new Point(236, 300), 100.5f, rand));

            playersSpawns.Add(new PlayerSpawnArea(new Point(212, 313), new Point(212, 313), 100.5f, rand));

            playersSpawns.Add(new PlayerSpawnArea(new Point(186, 300), new Point(186, 300), 100.5f, rand));

            playersSpawns.Add(new PlayerSpawnArea(new Point(210, 285), new Point(186, 300), 100.5f, rand));
        }

        public void ReviveDeadPlayers()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            foreach (KeyValuePair<int, Player> entry in gameState.players)
            {
                var player = entry.Value;
                if (player.dead && player.timeOfDeath + timeToRespawn < currentTime)
                {
                    player.Respawn(GetRandomSpawnPosition());
                }
            }
        }

        public Vector3 GetRandomSpawnPosition()
        {
            return playersSpawns.ElementAt(rand.Next(0, playersSpawns.Count)).GetRandomPosInArea();
        }

    }
}
