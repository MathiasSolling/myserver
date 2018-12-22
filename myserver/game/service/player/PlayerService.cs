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

        public void UpdatePlayer(Player player, Dictionary<int, float> actions)
        {
            foreach (KeyValuePair<int, float> entry in actions)
            {
                int psaKey = entry.Key;
                float psaValue = entry.Value;

                PlayerStateActionEnum psaEnum = (PlayerStateActionEnum)psaKey;
                if (psaEnum != PlayerStateActionEnum.PackageSeqNum)
                {
                    player.NewPsaKeyValue[psaKey] = psaValue;
                    switch (psaEnum)
                    {
                        case PlayerStateActionEnum.PosX:
                            player.PositionX = psaValue;
                            break;

                        case PlayerStateActionEnum.PosY:
                            player.PositionY = psaValue;
                            break;

                        case PlayerStateActionEnum.PosZ:
                            player.PositionZ = psaValue;
                            break;

                        case PlayerStateActionEnum.RotX:
                            player.RotationX = (int)psaValue;
                            break;

                        case PlayerStateActionEnum.RotY:
                            player.RotationY = (int)psaValue;
                            break;

                        case PlayerStateActionEnum.RotZ:
                            player.RotationZ = (int)psaValue;
                            break;

                        case PlayerStateActionEnum.VelocityX:
                            player.VelocityX = psaValue;
                            break;

                        case PlayerStateActionEnum.VelocityY:
                            player.VelocityY = psaValue;
                            break;

                        case PlayerStateActionEnum.VelocityZ:
                            player.VelocityZ = psaValue;
                            break;

                        case PlayerStateActionEnum.Jump:
                            player.Jump = psaValue == 1;
                            break;

                        case PlayerStateActionEnum.Shoot:
                            player.Shoot = psaValue == 1;
                            break;

                        case PlayerStateActionEnum.Aim:
                            player.Aim = psaValue == 1;
                            break;

                        case PlayerStateActionEnum.Run:
                            player.Run = psaValue == 1;
                            break;

                        case PlayerStateActionEnum.Crouch:
                            player.Crouch = psaValue == 1;
                            break;

                        case PlayerStateActionEnum.ShotPlayer:
                            ShotPlayer(player, (int)psaValue);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public void ShotPlayer(Player shooter, int targetId)
        {
            IKillable target = FindTargetById(targetId);
            
            if (target == null) return;
            Logger.Log("past 1", ActivityLogEnum.CRITICAL);
            if (shooter.Dead) return;
            Logger.Log("past 2", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon == null) return;
            Logger.Log("past 3", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon.BulletsInMag <= 0) return;
            Logger.Log("past 4", ActivityLogEnum.CRITICAL);
            if (shooter.playerId == targetId) return;
            Logger.Log("past 5", ActivityLogEnum.CRITICAL);

            int damage = shooter.ActiveWeapon.WeaponType.Damage;
            shooter.DamageDealtToPlayers += damage;

            bool targetDied = target.TakeDamage(damage, shooter.playerId);
            if (targetDied)
            {
                shooter.Kills++;
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
            if (gameState.zombies.TryGetValue(targetId, out Zombie zombie))
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
