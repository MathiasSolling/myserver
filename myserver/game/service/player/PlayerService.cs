using myserver.game.activitylog;
using myserver.game.gamelogic;
using myserver.game.npc.zombie;
using myserver.game.service.npc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    class PlayerService
    {
        private static ActivityLog Logger = new ActivityLog("PlayerService");

        private GameState gameState;

        public PlayerService(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void UpdatePlayer(Player player, Dictionary<PlayerStateActionEnum, float> actions)
        {
            foreach (KeyValuePair<PlayerStateActionEnum, float> entry in actions)
            {
                bool addKeyValueToNewPsaKeyValue = true;
                PlayerStateActionEnum psaEnum = entry.Key;
                float psaValue = entry.Value;
                
                switch (psaEnum)
                {
                    case PlayerStateActionEnum.PosX:
                        player.positionX = psaValue;
                        break;

                    case PlayerStateActionEnum.PosY:
                        player.positionY = psaValue;
                        break;

                    case PlayerStateActionEnum.PosZ:
                        player.positionZ = psaValue;
                        break;

                    case PlayerStateActionEnum.RotX:
                        player.rotationX = psaValue;
                        break;

                    case PlayerStateActionEnum.RotY:
                        player.rotationY = psaValue;
                        break;

                    case PlayerStateActionEnum.RotZ:
                        player.rotationZ = psaValue;
                        break;

                    case PlayerStateActionEnum.VelocityX:
                        player.velocityX = psaValue;
                        break;

                    case PlayerStateActionEnum.VelocityY:
                        player.velocityY = psaValue;
                        break;

                    case PlayerStateActionEnum.VelocityZ:
                        player.velocityZ = psaValue;
                        break;

                    case PlayerStateActionEnum.ShootRotationX:
                        player.shootRotationX = psaValue;
                        break;

                    case PlayerStateActionEnum.Jump:
                        player.jump = psaValue == 1;
                        break;

                    case PlayerStateActionEnum.Shoot:
                        player.shoot = psaValue == 1;
                        break;

                    case PlayerStateActionEnum.Aim:
                        player.aim = psaValue == 1;
                        break;

                    case PlayerStateActionEnum.Run:
                        player.run = psaValue == 1;
                        break;

                    case PlayerStateActionEnum.Crouch:
                        player.crouch = psaValue == 1;
                        break;

                    case PlayerStateActionEnum.ShotPlayer:
                        ShotPlayer(player, (int)psaValue);
                        addKeyValueToNewPsaKeyValue = false;
                        break;

                    default:
                        // Missing implementation of key given from player (Or player is cheating and sending custom keys)
                        addKeyValueToNewPsaKeyValue = false;
                        break;
                }
                if (addKeyValueToNewPsaKeyValue)
                {
                    player.NewPsaKeyValue[(int)psaEnum] = psaValue;
                }
            }
        }

        public void ShotPlayer(Player shooter, int targetId)
        {
            IKillable target = FindTargetById(targetId);

            if (target == null) return;
            Logger.Log("past 1", ActivityLogEnum.CRITICAL);
            if (target.IsDead()) return;
            Logger.Log("past 2", ActivityLogEnum.CRITICAL);
            if (shooter.dead) return;
            Logger.Log("past 3", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon == null) return;
            Logger.Log("past 4", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon.BulletsInMag <= 0) return;
            Logger.Log("past 5", ActivityLogEnum.CRITICAL);
            if (shooter.playerId == targetId) return;
            Logger.Log("past 6", ActivityLogEnum.CRITICAL);

            int damage = shooter.ActiveWeapon.WeaponType.Damage;
            Logger.Log("Weapon " + shooter.ActiveWeapon.WeaponType.WeaponName + " did " + damage + " damage to target", ActivityLogEnum.CRITICAL);
            shooter.damageDealtToPlayers += damage;

            bool targetDied = target.TakeDamage(damage, shooter.playerId);
            if (targetDied)
            {
                shooter.kills++;
                shooter.AddNewPsaKeyValue(PlayerStateActionEnum.Kills, shooter.kills);
            }

            shooter.ActiveWeapon.BulletsInMag = shooter.ActiveWeapon.BulletsInMag - 1;

            Logger.Log("Player#" + shooter.playerId + " shot Player#" + targetId, ActivityLogEnum.NORMAL);
        }

        public IKillable FindTargetById(int targetId)
        {
            if (gameState.players.TryGetValue(targetId, out Player player))
            {
                return player;
            }
            else if (gameState.zombies.TryGetValue(targetId, out Zombie zombie))
            {
                return zombie;
            }
            return null;
        }

        public string ConstructAllPlayerPostitions()
        {
            string playerPositions = "";
            foreach (KeyValuePair<int, Player> entry in gameState.players)
            {
                playerPositions += entry.Value.RetrieveNewPlayerState();
            }
            return playerPositions;
        }
    }
}
