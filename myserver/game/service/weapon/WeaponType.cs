using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.weapon
{
    class WeaponType
    {
        public string WeaponName;
        public int Damage;
        public int FireSpeed;

        public int MaxBulletsInMag;

        public int DropChanceOf1000;

        public bool Enabled;

        public WeaponType(string weaponName, int damage, int fireSpeed, int maxBulletsInMag, int dropChanceOf1000, bool enabled)
        {
            this.WeaponName = weaponName;
            this.Damage = damage;
            this.FireSpeed = fireSpeed;
            this.MaxBulletsInMag = maxBulletsInMag;
            this.DropChanceOf1000 = dropChanceOf1000;
            this.Enabled = enabled;
        }
    }
}
