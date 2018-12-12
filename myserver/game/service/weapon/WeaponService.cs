using myserver.game.activitylog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace myserver.game.service.weapon
{
    class WeaponService
    {
        private static ActivityLog Logger = new ActivityLog("WeaponService");

        private int maxAmountOfWeaponsPerPlayer = 4;

        // Random object to pass around (If creating multiple new ones in a loop it will rand the same numbers)
        private Random rand = new Random();

        // List to keep weapons that are spawnable/enabled
        public ConcurrentBag<WeaponType> WeaponTypes = new ConcurrentBag<WeaponType>();

        // List to store weapons that has been spawned
        public ConcurrentBag<Weapon> AvailableWeapons = new ConcurrentBag<Weapon>();

        // Weapon spawn points on map
        public ConcurrentBag<WeaponSpawnArea> SpawnPoints = new ConcurrentBag<WeaponSpawnArea>();

        public WeaponService()
        {
            Init();
        }

        private void Init()
        {
            WeaponTypes.Add(new WeaponType(WeaponTypeNameEnum.FreeHands.ToString(), 10, 0, 1000000000, 0, false, true, true, false));
            WeaponTypes.Add(new WeaponType(WeaponTypeNameEnum.SniperRifle.ToString(), 25, 20, 1000000000, 1000, true, true, true, true));

            // Todo add weapon spawn points
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            SpawnPoints.Add(new WeaponSpawnArea(new Point(5, 15), new Point(15, 25), 2, rand));
            // ^ Random spawn points for testing purpose

            SpawnWeaponsOnMap();
            Logger.Log(AvailableWeapons.Count + " weapons spawned", ActivityLogEnum.NORMAL);
        }

        public bool PickUpWeapon(Player player, int weaponId)
        {
            bool canPickUpWeapon = false;
            if (player.Weapons.Count < maxAmountOfWeaponsPerPlayer)
            {
                foreach (var weapon in AvailableWeapons)
                {
                    if (weapon.WeaponId == weaponId)
                    {
                        player.Weapons.Add(weapon);
                        canPickUpWeapon = true;
                    }
                }
            }
            // todo Tell other players that this weapon has been picked up UDP + TCP
            return canPickUpWeapon;
        }

        public bool DropWeapon(Player player, int weaponId)
        {
            bool canDropWeapon = false;
            Weapon weap = player.Weapons.Find(x => x.WeaponId == weaponId);
            if (weap != null)
            {
                player.Weapons.Remove(weap);
                AvailableWeapons.Add(weap);
                canDropWeapon = true;
            }
            // todo Tell other players that this weapon has been dropped UDP + TCP
            return canDropWeapon;
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
            // todo Tell other players that this weapon has been swapped UDP + TCP
            return canSwitch;
        }

        public void ReloadWeapon(Player player)
        {
            player.ActiveWeapon.BulletsInMag = player.ActiveWeapon.WeaponType.MaxBulletsInMag;
            // todo Tell other players that this player is reloading UDP
        }

        public List<Weapon> SpawnWeaponsOnMap()
        {
            List<Weapon> spawnedWeaps = new List<Weapon>();
            foreach (var spawnPoint in SpawnPoints)
            {
                foreach (var weaponType in WeaponTypes)
                {
                    int randomNum = rand.Next(0, 1001);
                    if (randomNum < weaponType.DropChanceOf1000)
                    {
                        Vector3 spwanPosition = spawnPoint.GetRandomPosInArea();
                        Weapon newWeap = new Weapon(AvailableWeapons.Count + 1, weaponType, (int)spwanPosition.X, (int)spwanPosition.Y, (int)spwanPosition.Z);
                        AvailableWeapons.Add(newWeap);
                    }
                }
            }
            return spawnedWeaps;
        }

        public void CreateWeaponsForNewPlayer(Player player)
        {
            foreach (WeaponType weaponType in WeaponTypes)
            {
                if (weaponType.PlayerStarterWeapon)
                {
                    Weapon newWeap = new Weapon(AvailableWeapons.Count + 1, weaponType);
                    player.Weapons.Add(newWeap);
                    if (weaponType.PlayerStarterActiveWeapon)
                    {
                        player.ActiveWeapon = newWeap;
                    }
                }
            }
            if (player.ActiveWeapon == null)
            {
                Logger.Log("CreateWeaponsForNewPlayer() didn't give player an active weapon", ActivityLogEnum.CRITICAL);
            }
        }

        public WeaponType FindWeaponTypeByName(WeaponTypeNameEnum weaponTypeNameEnum)
        {
            foreach (var weaponType in WeaponTypes)
            {
                if (weaponType.WeaponName == weaponTypeNameEnum.ToString())
                {
                    return weaponType;
                }
            }
            return null;
        }
    }
}
