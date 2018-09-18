using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service
{
    class BaseSpawnArea
    {
        private Random rand;
        protected Rect spawnArea;
        protected float posY;

        protected BaseSpawnArea(Point a, Point b, float y, Random rand)
        {
            // Example on two points given to form a rect as a spawn area:
            // Point a = new Point(5, 15);
            // Point b = new Point(15, 25);
            this.spawnArea = new Rect(a, b);
            this.posY = y;
            this.rand = rand;
        }

        protected Vector3 GetRandomPosInArea()
        {
            double saX = spawnArea.X;
            double saY = spawnArea.Y;
            double x = rand.Next((int)saX, (int)(saX + spawnArea.Width));
            double z = rand.Next((int)saY, (int)(saY + spawnArea.Height));
            return new Vector3((float)x, posY, (float)z);
        }
    }
}
