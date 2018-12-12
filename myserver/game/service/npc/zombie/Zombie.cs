using myserver.game.service.npc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.npc.zombie
{
    class Zombie : Npc, IKillable
    {
        public float health = 200;
        private int damage = 20;
        private int moveSpeed = 4;

        private float attackRange = 1.5f;
        private float attacksPerSecond = 0.5f;
        private long lastTimeAttacked = 0;

        public Zombie(int npcId) : base(npcId, NpcTypeEnum.Zombie)
        {

        }

        public void MoveAndRotate(float deltaTime)
        {
            if (Target == null) return;
            Vector3 targetPos = new Vector3(Target.PositionX, Target.PositionY, Target.PositionZ);
            Vector3 newDir = targetPos - position;
            float distance = Vector3.Distance(position, targetPos);
            Vector3 newDirection = newDir / distance;

            double magnitude = Math.Sqrt(newDir.X * newDir.X + newDir.Y * newDir.Y + newDir.Z * newDir.Z);
            Vector3 delta = newDir / (float)magnitude;
            Vector3 velocity = delta * moveSpeed * deltaTime;

            if (Vector3.Distance(targetPos, position) > attackRange)
            {
                // Only move if not in attack range 
                position = position + (direction * moveSpeed * deltaTime);
                AddNewNsaKeyValue(PlayerStateActionEnum.PosX, position.X);
                AddNewNsaKeyValue(PlayerStateActionEnum.PosY, position.Y);
                AddNewNsaKeyValue(PlayerStateActionEnum.PosZ, position.Z);

                AddNewNsaKeyValue(PlayerStateActionEnum.VelocityX, velocity.X);
                //AddNewNsaKeyValue(PlayerStateActionEnum.VelocityY, velocity.Y);
                AddNewNsaKeyValue(PlayerStateActionEnum.VelocityZ, velocity.Z);
            }
            
            Vector3 rotAxis = Vector3.Cross(direction, newDirection);

            double rotAngle = Math.Acos(Vector3.Dot(direction, newDirection));

            Quaternion q = new Quaternion(rotAxis, (float)rotAngle);
            rotation = rotation * q;

            direction = newDirection;

            // TODO Above rotation calculations doesn't work

            //AddNewNsaKeyValue(PlayerStateActionEnum.RotX, rotation.X);
            //AddNewNsaKeyValue(PlayerStateActionEnum.RotY, rotation.Y);
            //AddNewNsaKeyValue(PlayerStateActionEnum.RotZ, rotation.Z);
        }

        public void UpdateNpcTarget(ConcurrentBag<Player> players)
        {
            Vector3 targetPos = new Vector3();
            if (Target != null)
            {
                targetPos = new Vector3(Target.PositionX, Target.PositionY, Target.PositionZ);
            }
            if (Target.Dead)
            {
                Target = null;
            }

            foreach (var player in players)
            {
                if (player.Dead) continue;
                if (Target == null)
                {
                    Target = player;
                    targetPos = new Vector3(Target.PositionX, Target.PositionY, Target.PositionZ);
                    AddNewNsaKeyValue(PlayerStateActionEnum.NpcTarget, Target.PlayerId);
                    continue;
                }
                Vector3 playerPos = new Vector3(player.PositionX, player.PositionY, player.PositionZ);
                if (Vector3.Distance(playerPos, position) < Vector3.Distance(targetPos, position))
                {
                    Target = player;
                    targetPos = new Vector3(Target.PositionX, Target.PositionY, Target.PositionZ);
                    AddNewNsaKeyValue(PlayerStateActionEnum.NpcTarget, Target.PlayerId);
                }
            }
        }

        public void AttackTarget()
        {
            if (Target == null) return;
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if ((1000 / attacksPerSecond) + lastTimeAttacked > currentTime)
            {
                return;
            }
            lastTimeAttacked = currentTime;
            if (Target.Dead)
            {
                Target = null;
                return;
            }

            Vector3 targetPos = new Vector3(Target.PositionX, Target.PositionY, Target.PositionZ);
            if (Vector3.Distance(targetPos, position) < attackRange)
            {
                Target.TakeDamage(damage, npcId);
            }
        }

        public bool TakeDamage(int damage, float attackerId)
        {
            if (dead) return false;
            health -= damage;
            if (health <= 0)
            {
                dead = true;
                AddNewNsaKeyValue(PlayerStateActionEnum.KilledBy, attackerId);
            }
            else
            {
                AddNewNsaKeyValue(PlayerStateActionEnum.Health, health);
            }
            return dead;
        }
    }
}
