using Newtonsoft.Json;
using System;
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

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int PositionZ { get; set; }

        public int RotationX { get; set; }
        public int RotationY { get; set; }
        public int RotationZ { get; set; }

        public bool W { get; set; } = false;
        public bool A { get; set; } = false;
        public bool S { get; set; } = false;
        public bool D { get; set; } = false;

        public bool Jump { get; set; } = false;
        public bool Shoot { get; set; } = false;
        public bool Aim { get; set; } = false;
        public bool Run { get; set; } = false;

        public bool Crouch { get; set; } = false;

        // Dictionary to hold new state and actions from client - these we broadcast from server to all clients
        public Dictionary<int, int> NewPsaKeyValue = new Dictionary<int, int>();

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
    }
}
