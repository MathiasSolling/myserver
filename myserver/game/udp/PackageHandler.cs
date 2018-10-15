using myserver.game.activitylog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.udp
{
    class PackageHandler
    {
        private static ActivityLog Logger = new ActivityLog("PackageHandler");
        private ConcurrentBag<Player> Players;

        public PackageHandler(ConcurrentBag<Player> players)
        {
            this.Players = players;
        }

        public string[] GetPlayer(string message, out Player player)
        {
            // messageSplit[0] is the package type 001
            string[] messageSplit = message.Split(';');
            // messageSplit[1] is the playerId
            int pId = Int32.Parse(messageSplit[1]);
            // Skipping first which is message type (001 player state) and second which is pId
            string[] packageArray = messageSplit.Skip(2).ToArray();

            player = FindPlayerById(pId);
            return packageArray;
        }

        public bool GetPlayerStateInformation(string[] packageArray, Player player, out Dictionary<int, float> actions)
        {
            bool playerNeedsCorrection = false;
            actions = new Dictionary<int, float>();
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
                        break;
                    }
                    // Increment players package seq server side so it matches the package we got
                    player.PackageSeq = receivedPackageSeqNum;
                }
                else
                {
                    Logger.Log("Malicious package!", ActivityLogEnum.CRITICAL);
                    break;
                }
                
                foreach (var action in actionsArray)
                {
                    string[] actionKeyValue = action.Split(':');
                    if (actionKeyValue.Length != 2)
                    {
                        continue;
                    }
                    int psaKey = Int32.Parse(actionKeyValue[0]);
                    float psaValue = float.Parse(actionKeyValue[1]);
                    actions[psaKey] = psaValue;
                }
            }
            return playerNeedsCorrection;
        }

        private Player FindPlayerById(int playerId)
        {
            foreach (var player in Players)
            {
                if (playerId == player.PlayerId)
                {
                    return player;
                }
            }
            return null;
        }
    }
}
