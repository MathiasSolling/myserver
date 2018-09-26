using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    class PlayerService
    {
        public bool UpdatePlayerState(Player player, String[] packageArray)
        {
            bool playerNeedsCorrection = false;
            foreach (var package in packageArray)
            {
                string[] actionsArray = package.Split(',');
                // packages should (might be malicious) contain an Integer representing PlayerStateActionEnum followed by ':' then value
                // we know the first packageSplit should be package sequence number but make sure!
                string[] sequenceNumber = actionsArray[0].Split(':');
                if (sequenceNumber.Length != 2)
                {
                    break;
                }
                PlayerStateActionEnum psa = (PlayerStateActionEnum)Int32.Parse(sequenceNumber[0]);
                if (psa == PlayerStateActionEnum.PackageSeqNum)
                {
                    int receivedPackageSeqNum = Int32.Parse(sequenceNumber[1]);
                    int expectedPackageSeqNum = player.PackageSeq + 1;
                    if (receivedPackageSeqNum != expectedPackageSeqNum && receivedPackageSeqNum - 64 < expectedPackageSeqNum)
                    {
                        // If packageSeq isnt the next one then skip it to get the right order of the packages
                        continue;
                    }
                    else if (receivedPackageSeqNum - 64 > expectedPackageSeqNum)
                    {
                        // player client is sending packages with seq number way higher than we expect from it. Means we have lost some packages from client
                        // and probably need to correct it (rubberband)
                        playerNeedsCorrection = true;
                        Console.WriteLine("player id " + player.PlayerId + " is sending packages with too high seq number - sending correction package");
                        break;
                    }
                    // Increment players package seq server side so it matches the package we got
                    player.PackageSeq = receivedPackageSeqNum;
                }
                else
                {
                    Console.WriteLine("Malicious package!");
                    break;
                }

                foreach (var action in actionsArray)
                {
                    UpdatePlayer(player, action);
                }
            }
            return playerNeedsCorrection;
        }

        private void UpdatePlayer(Player player, string action)
        {
            string[] actionKeyValue = action.Split(':');
            if (actionKeyValue.Length != 2)
            {
                return;
            }
            int psaKey = Int32.Parse(actionKeyValue[0]);
            float psaValue = float.Parse(actionKeyValue[1]);

            PlayerStateActionEnum psaEnum = (PlayerStateActionEnum)psaKey;
            if (psaEnum != PlayerStateActionEnum.PackageSeqNum && psaEnum != PlayerStateActionEnum.PlayerId)
            {
                player.NewPsaKeyValue[psaKey] = psaValue;
                switch (psaEnum)
                {
                    case PlayerStateActionEnum.PosX:
                        player.PositionX = (int)psaValue;
                        break;

                    case PlayerStateActionEnum.PosY:
                        player.PositionY = (int)psaValue;
                        break;

                    case PlayerStateActionEnum.PosZ:
                        player.PositionZ = (int)psaValue;
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

                    default:
                        break;
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
                    playerState += "," + entry.Key + ":" + entry.Value;
                }
            }
            else
            {
                // This is just to test instantiation
                // playerState = ";" + player.PlayerId + ",8:1";
            }
            player.NewPsaKeyValue.Clear();
            return playerState;
        }

        public void ShootPlayer(Player shooter, Player taker)
        {
            if (taker.Dead) return;
            if (shooter.ActiveWeapon.BulletsInMag <= 0) return;
            int damage = shooter.ActiveWeapon.WeaponType.Damage;
            if (taker.Health - damage <= 0)
            {
                taker.Health = 0;
                taker.Dead = true;
                // todo Broadcast to every that taker is dead
            } 
            taker.Health -= damage;
            shooter.ActiveWeapon.BulletsInMag -= 1;
        }
    }
}
