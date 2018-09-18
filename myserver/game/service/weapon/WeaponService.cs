using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service.weapon
{
    class WeaponService
    {
        private int maxAmountOfWeaponsPerPlayer = 4;

        // Random object to pass around (If creating multiple new ones in a loop it will rand the same numbers)
        private Random rand = new Random();

        // List to keep weapons that are spawnable/enabled
        public List<Weapon> BaseWeapons = new List<Weapon>();

        // List to store weapons that has been spawned
        public List<Weapon> AvailableWeapons = new List<Weapon>();

        // Weapon spawn points on map
        public List<WeaponSpawnArea> SpawnPoints = new List<WeaponSpawnArea>();

        public WeaponService()
        {
            Init();
        }

        private void Init()
        {
            BaseWeapons.Add(new Weapon("FreeHands", 20, 0, true));
            BaseWeapons.Add(new Weapon("SniperRifle", 100, 20, true));

            // Todo add weapon spawn points
            
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            // ^ Random spawn points for testing purpose

            SpawnWeaponsOnMap();
        }

        public bool PickUpWeapon(Player player, int weaponId)
        {
            bool canPickUpWeapon = false;
            if (player.Weapons.Count < maxAmountOfWeaponsPerPlayer)
            {
                Weapon weap = AvailableWeapons.Find(x => x.WeaponId == weaponId);
                if (weap != null)
                {
                    player.Weapons.Add(weap);
                    canPickUpWeapon = true;
                }
            }
            return canPickUpWeapon;
        }

        public bool SwitchWeapon(Player player, int weaponId)
        {
            bool canSwitch = false;
            Weapon weap = player.Weapons.Find(x => x.WeaponId == weaponId);
            if (weap != null)
            {
                player.ActiveWeapon = weap;
                canSwitch = true;
            }
            return canSwitch;
        }

        public List<Weapon> SpawnWeaponsOnMap()
        {
            List<Weapon> spawnedWeaps = new List<Weapon>();
            foreach (var spawnPoint in SpawnPoints)
            {
                
            }
            return spawnedWeaps;
        }
    }
}
