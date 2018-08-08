﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myserver.game.udp
{
    class Broadcaster
    {

        private GameControlCenter gameControlCenter;

        // Amount of time between each broadcast 
        // Usally 50 which is 20 times a second, for development 2000 is good which is once every 2 seconds
        private long timeInMillisBeforeBroadcastingAgain = 45;

        private long timeLastIterationInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long timeStartedCountingTicksInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private int broadcastCount = 0;

        public Broadcaster(GameControlCenter gameControlCenter)
        {
            this.gameControlCenter = gameControlCenter;
        }

        public void BroadcastGameState()
        {
            while (true)
            {
                long timeNowInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                // Make sure that x ms has passed before broadcasting gamestate again
                if (timeNowInMillis - timeInMillisBeforeBroadcastingAgain >= timeLastIterationInMillis)
                {
                    double deltaTime = (timeNowInMillis - timeLastIterationInMillis) / 1000;
                    // Now broadcast gamestate to all players
                    // gameControlCenter.DoGameLogic(deltaTime);
                    gameControlCenter.BroadcastGameState();

                    timeLastIterationInMillis = timeNowInMillis;

                    // If more than 1000ms (1 sec) has passed since starting tickCount - then print and reset
                    if (timeNowInMillis - 1000 > timeStartedCountingTicksInMillis)
                    {
                        //Console.WriteLine("Broadcast count: " + broadcastCount);
                        broadcastCount = 1;
                        timeStartedCountingTicksInMillis = timeNowInMillis;
                    }
                    else
                    {
                        broadcastCount++;
                    }
                }
                else
                {
                    // Calculate how long until we are supposed to broadcast gamestate again and then put thread to sleep for that amount of time
                    // From 12% CPU to 0.1% CPU after implementing this
                    long sleepTime = timeNowInMillis - timeLastIterationInMillis + timeInMillisBeforeBroadcastingAgain;
                    if (sleepTime > 0L)
                    {
                        Thread.Sleep((int)sleepTime);
                    }
                }
            }
        }
    }
}