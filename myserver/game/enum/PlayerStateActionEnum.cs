using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    public enum PlayerStateActionEnum
    {
        PlayerId = 0,
        PackageSeqNum = 1,
        PosX = 2,
        PosY = 3,
        PosZ = 4,

        RotX = 5,
        RotY = 6,
        RotZ = 7,

        Jump = 8,
        Shoot = 9,
        Aim = 10,
        Run = 11,

        W = 12,
        S = 13,
        A = 14,
        D = 15,

        Crouch = 16
    }
}
