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

        public int WeaponPosX = 0;
        public int WeaponPosY = 0;
        public int WeaponPosZ = 0;

        public bool PickedUp = false;

        public Weapon(int weaponId, WeaponType weaponType, int x, int y, int z)
        {
            this.WeaponId = weaponId;
            this.WeaponType = weaponType;
            this.BulletsInMag = weaponType.MaxBulletsInMag;
            this.WeaponPosX = x;
            this.WeaponPosY = y;
            this.WeaponPosZ = z;
        }

        public Weapon(int weaponId, WeaponType weaponType)
        {
            this.WeaponId = weaponId;
            this.WeaponType = weaponType;
            this.BulletsInMag = weaponType.MaxBulletsInMag;
        }
    }
}
