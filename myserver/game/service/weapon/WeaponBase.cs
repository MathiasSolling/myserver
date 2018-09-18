using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.service.weapon
{
    class WeaponBase
    {
        public string WeaponName;
        public int Damage;
        public int FireSpeed;
        public bool Enabled;

        protected WeaponBase(string weaponName, int damage, int fireSpeed, bool enabled)
        {
            this.WeaponName = weaponName;
            this.Damage = damage;
            this.FireSpeed = fireSpeed;
            this.Enabled = enabled;
        }
    }
}
