﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.npc.zombie
{
    class Npc
    {
        public int npcId;
        public ObjectType objectType;

        public Vector3 direction = new Vector3(0, 0, 0);
        public Vector3 position = new Vector3(4, 12, 8);
        public Quaternion rotation = new Quaternion(0, 0, 0, 0);

        public Player Target = null;

        public bool dead = false;

        // State and actions for the clients to know about the npcs
        public ConcurrentDictionary<int, float> NewNsaKeyValue = new ConcurrentDictionary<int, float>();

        public Npc (int npcId, Vector3 startPos, ObjectType objectType)
        {
            this.npcId = npcId;
            this.position = startPos;
            this.objectType = objectType;
        }

        public String RetrieveNewState()
        {
            string npcState = "";
            if (NewNsaKeyValue.Count != 0)
            {
                npcState += ";";
                npcState += (int)PlayerStateActionEnum.ObjectId + ":" + npcId;
                npcState += "," + (int)PlayerStateActionEnum.ObjectType + ":" + (int)objectType;

                foreach (KeyValuePair<int, float> entry in NewNsaKeyValue)
                {
                    npcState += "," + entry.Key + ":" + entry.Value.ToString("0.##");
                }
                NewNsaKeyValue.Clear();
            }
            return npcState;
        }

        public void AddNewNsaKeyValue(PlayerStateActionEnum playerStateActionEnum, float value)
        {
            if (playerStateActionEnum == PlayerStateActionEnum.PackageSeqNum)
            {
                return;
            }
            NewNsaKeyValue[(int)playerStateActionEnum] = value;
        }

        protected int HealthLeftInPercentages(int maxHealth, int healthLeft)
        {
            return (healthLeft / maxHealth) * 100;
        }
    }
}
