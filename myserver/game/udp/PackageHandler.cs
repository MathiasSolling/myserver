using myserver.game.activitylog;
using myserver.game.gamelogic;
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
        private GameState gameState;

        public PackageHandler(GameState gameState)
        {
            this.gameState = gameState;
        }

        public string[] GetPlayer(string message, out Player player)
        {
            // messageSplit[0] is the package type 001
            string[] messageSplit = message.Split(';');
            // messageSplit[1] is the playerId
            if (!int.TryParse(messageSplit[1], out int pId))
            {
                player = null;
                return new string[0];
            }
            // Skipping first which is message type (001 player state) and second which is pId
            string[] packageArray = messageSplit.Skip(2).ToArray();

            player = gameState.FindPlayerById(pId);
            return packageArray;
        }

        public bool GetPlayerStateInformation(string[] packageArray, Player player, out Dictionary<PlayerStateActionEnum, float> actions)
        {
            bool playerNeedsCorrection = false;
            actions = new Dictionary<PlayerStateActionEnum, float>();
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
                else if (Int32.Parse(sequenceNumber[0]) != (int)PlayerStateActionEnum.PackageSeqNum)
                {
                    break;
                }

                // We know the value is packageSeqNumber
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

                // Validate actions and put them into dictionary
                foreach (var action in actionsArray)
                {
                    string[] actionKeyValue = action.Split(':');

                    if (actionKeyValue.Length != 2)
                    {
                        continue;
                    }

                    if (!Int32.TryParse(actionKeyValue[0], out int psaKey) || !float.TryParse(actionKeyValue[1], out float psaValue))
                    {
                        continue;
                    }
                    actions[(PlayerStateActionEnum) psaKey] = psaValue;
                }
            }
            return playerNeedsCorrection;
        }
    }
}
