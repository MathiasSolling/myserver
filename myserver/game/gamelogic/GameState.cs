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

        public ConcurrentBag<Player> players = new ConcurrentBag<Player>();
        public ConcurrentBag<Zombie> zombies = new ConcurrentBag<Zombie>();

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
    }
}
