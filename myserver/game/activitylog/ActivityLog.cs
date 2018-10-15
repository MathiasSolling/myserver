using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game.activitylog
{
    class ActivityLog
    {
        private string className;
        private static List<String> logList = new List<String>();

        public ActivityLog(string className){
            this.className = className;
        }

        public void Log(string message, ActivityLogEnum activityLogEnum)
        {
            string msg = className + " " + activityLogEnum + ": " + message;
            //logList.Add(msg);
            Console.WriteLine(msg);
        }

        public void Log(string message, int playerId, ActivityLogEnum activityLogEnum)
        {
            string msg = className + " " + activityLogEnum + ": " + message + " (playerId: " + playerId + " )";
            //logList.Add(msg);
            Console.WriteLine(msg);
        }
    }
}
