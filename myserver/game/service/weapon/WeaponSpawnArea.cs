using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service.weapon
{
    class WeaponSpawnArea : BaseSpawnArea
    {

        public WeaponSpawnArea(Point a, Point b, float y, Random rand) : base(a, b, y, rand)
        {
            // Example on two points given to form a rect as a spawn area:
            // Point a = new Point(5, 15);
            // Point b = new Point(15, 25);
        }

    }
}
