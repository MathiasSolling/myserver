using myserver.game.gamelogic;
using myserver.game.service.weapon;
using myserver.game.udp;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace myserver.game
{
    public class Game : IDisposable
    {
        private UdpListener udpListener;

        private UdpClient udpClient;
        
        private GameManager gameManager;

        private int UdpPort = 36200;

        private bool disposed = false;

        private GameState gameState;

        private int gameId = 1;

        public Game()
        {
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, UdpPort));
            gameState = new GameState(udpClient, gameId);
            gameManager = new GameManager(gameState);

            StartUdpServer();
        }

        void StartUdpServer()
        {
            // listen for incomming messages
            udpListener = new UdpListener(udpClient);
            udpListener.Listen();


            // Thread to process the incomming messages (Since network is faster than actual execution of code)
            HandleData handleData = new HandleData(gameManager);
            Thread handleDataThread = new Thread(() => handleData.subscribeToEvent(udpListener));
            handleDataThread.Start();

            // Thread to broadcast gamestate to all players in a fixed interval
            Broadcaster broadCaster = new Broadcaster(gameManager);
            Thread broadcasterThread = new Thread(() => broadCaster.BroadcastGameState());
            broadcasterThread.Start(); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (udpListener != null)
                    {
                        this.udpListener.Dispose();
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Game() {
            Dispose(false);
        }
    }
}