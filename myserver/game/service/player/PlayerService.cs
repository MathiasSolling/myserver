using myserver.game.activitylog;
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

        private ConcurrentBag<Player> Players;

        public PlayerService(ConcurrentBag<Player> Players)
        {
            this.Players = Players;
        }

        private Player FindPlayerById(int playerId)
        {
            Player playerObj = null;
            foreach (var player in Players)
            {
                if (player.PlayerId == playerId)
                {
                    playerObj = player;
                }
            }
            return playerObj;
        }

        public void UpdatePlayer(Player player, Dictionary<int, float> actions)
        {
            foreach (KeyValuePair<int, float> entry in actions)
            {
                int psaKey = entry.Key;
                float psaValue = entry.Value;

                PlayerStateActionEnum psaEnum = (PlayerStateActionEnum)psaKey;
                if (psaEnum != PlayerStateActionEnum.PackageSeqNum && psaEnum != PlayerStateActionEnum.PlayerId)
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

        public String RetrieveNewPlayerState(Player player)
        {
            String playerState = "";
            if (player.NewPsaKeyValue.Count != 0)
            {
                playerState = ";" + player.PlayerId;
                foreach (KeyValuePair<int, float> entry in player.NewPsaKeyValue)
                {
                    playerState += "," + entry.Key + ":" + entry.Value.ToString("0.##");
                }
            }
            player.NewPsaKeyValue.Clear();
            return playerState;
        }

        public void ShotPlayer(Player shooter, int playerShotId)
        {
            Player playerShot = FindPlayerById(playerShotId);
            
            if (playerShot == null) return;
            Logger.Log("past 1", ActivityLogEnum.CRITICAL);
            if (playerShot.Dead) return;
            Logger.Log("past 2", ActivityLogEnum.CRITICAL);
            if (shooter.Dead) return;
            Logger.Log("past 3", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon == null) return;
            Logger.Log("past 4", ActivityLogEnum.CRITICAL);
            if (shooter.ActiveWeapon.BulletsInMag <= 0) return;
            Logger.Log("past 5", ActivityLogEnum.CRITICAL);
            if (shooter.PlayerId == playerShotId) return;
            Logger.Log("past 6", ActivityLogEnum.CRITICAL);

            int damage = shooter.ActiveWeapon.WeaponType.Damage;
            shooter.DamageDealtToPlayers += damage;

            if (playerShot.Health - damage <= 0)
            {
                playerShot.Health = 0;
                playerShot.Dead = true;
                playerShot.AddNewPsaKeyValue(PlayerStateActionEnum.KilledBy, shooter.PlayerId);

                shooter.Kills++;
            }
            else
            {
                playerShot.Health = playerShot.Health - damage;
                playerShot.AddNewPsaKeyValue(PlayerStateActionEnum.Health, playerShot.Health);
            }
            
            shooter.ActiveWeapon.BulletsInMag = shooter.ActiveWeapon.BulletsInMag - 1;

            Logger.Log("Player#" + shooter.PlayerId + " shot Player#" + playerShot.PlayerId, ActivityLogEnum.NORMAL);
        }
    }
}
