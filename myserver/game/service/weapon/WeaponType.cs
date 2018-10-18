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

        public bool DropAble;

        public bool Enabled;

        // Bool to identify if player should have this weapon when starting game
        public bool PlayerStarterWeapon;
        // Bool to identify if this should be the active weapon from start
        public bool PlayerStarterActiveWeapon;

        public WeaponType(string weaponName, int damage, int fireSpeed, int maxBulletsInMag, int dropChanceOf1000, bool dropAble, bool enabled, bool playerStarterWeapon, bool playerStarterActiveWeapon)
        {
            this.WeaponName = weaponName;
            this.Damage = damage;
            this.FireSpeed = fireSpeed;
            this.MaxBulletsInMag = maxBulletsInMag;
            this.DropChanceOf1000 = dropChanceOf1000;
            this.DropAble = dropAble;
            this.Enabled = enabled;
            this.PlayerStarterWeapon = playerStarterWeapon;
            this.PlayerStarterActiveWeapon = playerStarterActiveWeapon;
        }
    }
}
