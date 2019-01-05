using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service.gametype.deathmatch
{
    class PlayerSpawnArea : BaseSpawnArea
    {
        public PlayerSpawnArea(Point a, Point b, float y, Random rand) : base(a, b, y, rand)
        {

        }
    }
}
