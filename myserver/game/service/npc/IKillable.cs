using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.npc
{
    interface IKillable
    {
        bool TakeDamage(int damage, float attackerId);
    }
}
