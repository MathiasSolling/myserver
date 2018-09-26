using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myserver.game
{
    public enum PlayerStateActionEnum
    {
        Unknown = 0,

        PlayerId,
        PackageSeqNum,
        PosX,
        PosY,
        PosZ,

        RotX,
        RotY,
        RotZ,

        Jump,
        Shoot,
        Aim,
        Run,

        VelocityX,
        VelocityY,
        VelocityZ,

        Crouch,

        PickUpWeapon,
        DropWeapon,
        SwitchWeapon
    }
}
