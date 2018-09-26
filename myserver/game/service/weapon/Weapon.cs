using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.weapon
{
    class Weapon
    {
        public int WeaponId;

        public WeaponType WeaponType;

        public int BulletsInMag;

        public int WeaponPosX;
        public int WeaponPosY;
        public int WeaponPosZ;

        public bool PickedUp = false;

        public Weapon(int weaponId, WeaponType weaponType, int BulletsInMag, int x, int y, int z)
        {
            this.WeaponId = weaponId;
            this.WeaponType = weaponType;
            this.WeaponPosX = x;
            this.WeaponPosY = y;
            this.WeaponPosZ = z;
        }
    }
}
