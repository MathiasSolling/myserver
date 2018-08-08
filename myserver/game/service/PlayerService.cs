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
                string[] packageSplit = package.Split(',');
                // packages should(might be malicious) contain an Integer representing PlayerStateActionEnum followed by ':' then value
                // we know the first packageSplit should be package sequence number but make sure!
                PlayerStateActionEnum psa = (PlayerStateActionEnum)Int32.Parse(packageSplit[0].Split(':')[0]);
                if (psa == PlayerStateActionEnum.PackageSeqNum)
                {
                    int receivedPackageSeqNum = Int32.Parse(packageSplit[0].Split(':')[1]);
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

                foreach (var action in packageSplit)
                {
                    string[] actionKeyValue = action.Split(':');
                    int psaKey = Int32.Parse(actionKeyValue[0]);
                    int psaValue = Int32.Parse(actionKeyValue[1]);

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
                                player.RotationX = psaValue;
                                break;

                            case PlayerStateActionEnum.RotY:
                                player.RotationY = psaValue;
                                break;

                            case PlayerStateActionEnum.RotZ:
                                player.RotationZ = psaValue;
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

                            default:
                                break;
                        }
                    }
                }
            }
            return playerNeedsCorrection;
        }

        public String RetrieveNewPlayerState(Player player)
        {
            String playerState = "";
            if (player.NewPsaKeyValue.Count != 0)
            {
                playerState = ";" + player.PlayerId;
                foreach (KeyValuePair<int, int> entry in player.NewPsaKeyValue)
                {
                    playerState += "," + entry.Key + ":" + entry.Value;
                }
            }
            player.NewPsaKeyValue.Clear();
            return playerState;
        }
    }
}
