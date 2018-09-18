using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.weapon
{
    class Weapon : WeaponBase
    {
        public int WeaponId;

        public int WeaponPosX;
        public int WeaponPosY;
        public int WeaponPosZ;

        public bool PickedUp = false;

        public Weapon(string weaponName, int damage, int fireSpeed, bool enabled) : base(weaponName,  damage, fireSpeed, enabled)
        {

        }
    }
}
