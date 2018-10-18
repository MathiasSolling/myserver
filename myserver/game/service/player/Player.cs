using myserver.game.service.weapon;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    class Player
    {
        public int PlayerId { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public int RotationX { get; set; }
        public int RotationY { get; set; }
        public int RotationZ { get; set; }

        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float VelocityZ { get; set; }

        public bool Jump { get; set; } = false;
        public bool Shoot { get; set; } = false;
        public bool Aim { get; set; } = false;
        public bool Run { get; set; } = false;

        public bool Crouch { get; set; } = false;

        public bool Dead = false;

        public int Health = 500;

        public List<Weapon> Weapons = new List<Weapon>();
        public Weapon ActiveWeapon;

        public int DamageDealtToNpc = 0;
        public int DamageDealtToPlayers = 0;

        public int Kills = 0;

        // Dictionary to hold new state and actions from client - these we broadcast from server to all clients
        public ConcurrentDictionary<int, float> NewPsaKeyValue = new ConcurrentDictionary<int, float>();

        [JsonIgnore]
        public long LastTimeShotFiredInMillis { get; set; } = 0;

        [JsonIgnore]
        public int PackageSeq { get; set; } = 1;

        [JsonIgnore]
        public IPEndPoint Ep { get; set; }

        public Player()
        {

        }

        public Player(int playerId, int posX, int posY, int posZ, int rotX, int rotY, int rotZ, IPEndPoint ep)
        {
            this.PlayerId = playerId;

            this.PositionX = posX;
            this.PositionY = posY;
            this.PositionZ = posZ;

            this.RotationX = rotX;
            this.RotationY = rotY;
            this.RotationZ = rotZ;

            this.Ep = ep;
        }

        public void AddNewPsaKeyValue(PlayerStateActionEnum playerStateActionEnum, float value)
        {
            if (playerStateActionEnum == PlayerStateActionEnum.PackageSeqNum || playerStateActionEnum == PlayerStateActionEnum.PlayerId)
            {
                return;
            }
            NewPsaKeyValue[(int)playerStateActionEnum] = value;
        }
    }
}
