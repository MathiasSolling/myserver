using myserver.game.activitylog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myserver.game.udp
{
    class Broadcaster
    {
        private static ActivityLog Logger = new ActivityLog("Broadcaster");
        private GameManager gameManager;
        
        // Broadcast count
        private long ticks = 60;

        private long timeLastIterationInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private long timeStartedCountingTicksInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        private int broadcastCount = 0;

        public Broadcaster(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void BroadcastGameState()
        {
            while (true)
            {
                long timeNowInMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                // Make sure that x ms has passed before broadcasting gamestate again
                decimal timePassed = timeNowInMillis - timeLastIterationInMillis;
                decimal delta = 1000 / ticks;
                if (timePassed > delta)
                {
                    float deltaTime = (timeNowInMillis - timeLastIterationInMillis) / 1000f;
                    // Now broadcast gamestate to all players
                    gameManager.BroadcastGameState();
                    // Do game logic after broadcast so it doesnt block broadcast wait time
                    gameManager.DoGameLogic(deltaTime);

                    timeLastIterationInMillis = timeNowInMillis;

                    // If more than 1000ms (1 sec) has passed since starting tickCount - then print and reset
                    if (timeNowInMillis - 1000 > timeStartedCountingTicksInMillis)
                    {
                        // Logger.Log("Broadcast count: " + broadcastCount, ActivityLogEnum.NORMAL);
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
                    long sleepTime = timeNowInMillis - timeLastIterationInMillis - (long)delta;
                    if (sleepTime > 0L)
                    {
                        Thread.Sleep((int)sleepTime);
                    }
                }
            }
        }
    }
}
