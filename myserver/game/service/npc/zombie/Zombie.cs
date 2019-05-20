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
        private int maxHealth = 200;
        private int health;
        private int damage = 10;
        private int moveSpeed = 4;

        private float attackRange = 5f;
        private double attacksPerSecond = 0.5;
        private long lastTimeAttacked = 0;

        public Zombie(int npcId, Vector3 startPos) : base(npcId, startPos, ObjectType.Zombie)
        {
            health = maxHealth;
        }

        public void MoveAndRotate(float deltaTime)
        {
            if (Target == null) return;
            Vector3 targetPos = new Vector3(Target.positionX, Target.positionY, Target.positionZ);
            Vector3 newDir = targetPos - position;
            float distance = Vector3.Distance(position, targetPos);
            Vector3 newDirection = newDir / distance;

            double magnitude = Math.Sqrt(newDir.X * newDir.X + newDir.Y * newDir.Y + newDir.Z * newDir.Z);
            Vector3 delta = newDir / (float)magnitude;
            Vector3 velocity = delta * moveSpeed * deltaTime;

            // Only move if not in attack range
            if (Vector3.Distance(targetPos, position) > attackRange / 2)
            {
                position = position + (direction * moveSpeed * deltaTime);

                AddNewNsaKeyValue(PlayerStateActionEnum.PosX, position.X);
                AddNewNsaKeyValue(PlayerStateActionEnum.PosY, position.Y);
                AddNewNsaKeyValue(PlayerStateActionEnum.PosZ, position.Z);
                AddNewNsaKeyValue(PlayerStateActionEnum.TargetId, Target.playerId);
            }
            direction = newDirection;
        }

        public void UpdateNpcTarget(ConcurrentDictionary<int, Player> players)
        {
            Vector3 targetPos = new Vector3();
            if (Target != null)
            {
                if (Target.dead)
                {
                    Target = null;
                    AddNewNsaKeyValue(PlayerStateActionEnum.TargetId, -1);
                }
                else
                {
                    targetPos = new Vector3(Target.positionX, Target.positionY, Target.positionZ);
                }
            }

            foreach (KeyValuePair<int, Player> entry in players)
            {
                var player = entry.Value;
                if (player.dead) continue;
                if (Target == null)
                {
                    Target = player;
                    targetPos = new Vector3(Target.positionX, Target.positionY, Target.positionZ);
                    continue;
                }
                Vector3 playerPos = new Vector3(player.positionX, player.positionY, player.positionZ);
                if (Vector3.Distance(playerPos, position) < Vector3.Distance(targetPos, position))
                {
                    Target = player;
                    targetPos = new Vector3(Target.positionX, Target.positionY, Target.positionZ);
                }
            }
        }

        public void AttackTarget()
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (Target == null)
            {
                return;
            }
            else if ((1000 / attacksPerSecond) + lastTimeAttacked > currentTime)
            {
                return;
            }
            lastTimeAttacked = currentTime;
            if (Target.dead)
            {
                Target = null;
                return;
            }

            Vector3 targetPos = new Vector3(Target.positionX, Target.positionY, Target.positionZ);
            if (Vector3.Distance(targetPos, position) < attackRange)
            {
                Target.TakeDamage(damage, npcId);
                AddNewNsaKeyValue(PlayerStateActionEnum.Attack, 1);
            }
        }

        public bool IsDead()
        {
            return dead;
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
                AddNewNsaKeyValue(PlayerStateActionEnum.Health, HealthLeftInPercentages(maxHealth, health));
            }
            return dead;
        }
    }
}
