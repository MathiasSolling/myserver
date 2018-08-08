﻿using myserver.game.udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myserver.game
{
    public class Game : IDisposable
    {
        private UdpListener udpListener;
        private GameControlCenter gameControlCenter = new GameControlCenter(1);

        private bool disposed = false;

        public Game()
        {
            StartUdpServer();
        }

        void StartUdpServer()
        {
            //create a new server
            Console.WriteLine("Listening to clients");

            // Thread to listen for incomming messages
            udpListener = new UdpListener(new IPEndPoint(IPAddress.Any, 9601));
            Thread listenerThread = new Thread(() => udpListener.Listen());
            listenerThread.Start();


            // Thread to process the incomming messages (Since network is faster than actual execution of code)
            HandleData handleData = new HandleData(gameControlCenter);
            Thread handleDataThread = new Thread(() => handleData.subscribeToEvent(udpListener));
            handleDataThread.Start();

            // Thread to broadcast gamestate to all players in a fixed interval
            Broadcaster broadCaster = new Broadcaster(gameControlCenter);
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