using myserver.game.npc.zombie;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.gamelogic
{
    class GameState
    {
        public UdpClient udpClient;

        public int gameId;

        public long gameStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long gameEndTime;

        public ConcurrentDictionary<int, Player> players = new ConcurrentDictionary<int, Player>();
        public ConcurrentDictionary<int, Zombie> zombies = new ConcurrentDictionary<int, Zombie>();

        private int UID = 0;
        private object _lock = new object();

        public GameState(UdpClient udpClient, int gameId)
        {
            this.udpClient = udpClient;
            this.gameId = gameId;
        }

        public void SetGameEndTime()
        {
            this.gameEndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public int GetNextUID()
        {
            // Thread safe
            lock (_lock)
            {
                return ++UID;
            }
        }

        public Player FindPlayerById(int playerId)
        {
            if (players.TryGetValue(playerId, out Player player))
            {
                return player;
            }
            return null;
        }
    }
}
