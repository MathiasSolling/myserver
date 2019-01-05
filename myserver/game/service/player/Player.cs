using myserver.game.service.npc;
using myserver.game.service.weapon;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    class Player : IKillable
    {
        public int playerId = 0;

        public float positionX = 0;
        public float positionY = 0;
        public float positionZ = 0;

        public int rotationX = 0;
        public int rotationY = 0;
        public int rotationZ = 0;

        public float velocityX = 0;
        public float velocityY = 0;
        public float velocityZ = 0;

        public float shootRotationX = 0f;

        public bool jump = false;
        public bool shoot = false;
        public bool aim = false;
        public bool run = false;

        public bool crouch = false;

        public bool dead = false;
        public long timeOfDeath = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public int maxHealth = 500;
        public int health;

        public List<Weapon> Weapons = new List<Weapon>();
        public Weapon ActiveWeapon;

        public int damageDealtToNpc = 0;
        public int damageDealtToPlayers = 0;

        public int kills = 0;

        // Dictionary to hold new state and actions sent from the player client to then broadcast right away to all other clients
        public ConcurrentDictionary<int, float> NewPsaKeyValue = new ConcurrentDictionary<int, float>();

        // Dictionary to hold history of player state
        public ConcurrentDictionary<long, ConcurrentDictionary<int, float>> psaKeyValueHistory = new ConcurrentDictionary<long, ConcurrentDictionary<int, float>>();

        [JsonIgnore]
        public long lastTimeShotFiredInMillis = 0;

        [JsonIgnore]
        public int packageSeq = 1;

        [JsonIgnore]
        public IPEndPoint Ep { get; set; }

        public long timeOfLoggedIn = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public Player(int playerId, int posX, int posY, int posZ, int rotX, int rotY, int rotZ, IPEndPoint ep)
        {
            this.playerId = playerId;

            this.positionX = posX;
            this.positionY = posY;
            this.positionZ = posZ;

            this.rotationX = rotX;
            this.rotationY = rotY;
            this.rotationZ = rotZ;

            this.Ep = ep;

            health = maxHealth;

            // Store player's first position in history of actions
            ArchiveStateBeforeClear();
        }

        public bool IsDead()
        {
            return dead;
        }

        public bool TakeDamage(int damage, float attackerId)
        {
            if (dead)
            {
                return false;
            }
            health = health - damage;
            if (health <= 0)
            {
                dead = true;
                timeOfDeath = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                AddNewPsaKeyValue(PlayerStateActionEnum.KilledBy, attackerId);
            }
            else
            {
                AddNewPsaKeyValue(PlayerStateActionEnum.Health, HealthLeftInPercentages());
            }
            return dead;
        }

        public string RetrieveNewPlayerState()
        {
            string playerState = "";
            if (NewPsaKeyValue.Count != 0)
            {
                playerState += ";";
                playerState += (int)PlayerStateActionEnum.ObjectId + ":" + playerId;
                playerState += "," + (int)PlayerStateActionEnum.ObjectType + ":" + (int)ObjectType.Player;

                foreach (KeyValuePair<int, float> entry in NewPsaKeyValue)
                {
                    playerState += "," + entry.Key + ":" + entry.Value.ToString("0.##");
                }
            }
            ArchiveStateBeforeClear();
            NewPsaKeyValue.Clear();
            return playerState;
        }

        private void ArchiveStateBeforeClear()
        {
            ConcurrentDictionary<int, float> newDict = new ConcurrentDictionary<int, float>();
            foreach (KeyValuePair<int, float> entry in NewPsaKeyValue)
            {
                newDict[entry.Key] = entry.Value;
            }

            // Make sure player position is always archived even if not changed
            newDict[(int)PlayerStateActionEnum.PosX] = positionX;
            newDict[(int)PlayerStateActionEnum.PosY] = positionY;
            newDict[(int)PlayerStateActionEnum.PosZ] = positionZ;

            long currTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            psaKeyValueHistory[currTime] = newDict;
        }

        public void AddNewPsaKeyValue(PlayerStateActionEnum playerStateActionEnum, float value)
        {
            if (playerStateActionEnum == PlayerStateActionEnum.PackageSeqNum)
            {
                return;
            }
            NewPsaKeyValue[(int)playerStateActionEnum] = value;
        }

        public int HealthLeftInPercentages()
        {
            decimal h = health;
            decimal mh = maxHealth;
            return (int)(h / mh * 100);
        }

        public void Respawn(Vector3 position)
        {
            dead = false;
            AddNewPsaKeyValue(PlayerStateActionEnum.Respawn, 1);

            timeOfDeath = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            health = maxHealth;

            positionX = position.X;
            AddNewPsaKeyValue(PlayerStateActionEnum.PosX, positionX);
            positionY = position.Y;
            AddNewPsaKeyValue(PlayerStateActionEnum.PosY, positionY);
            positionZ = position.Z;
            AddNewPsaKeyValue(PlayerStateActionEnum.PosZ, positionZ);
        }
    }
}
