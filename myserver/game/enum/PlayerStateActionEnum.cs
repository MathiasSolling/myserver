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

        // Floats
        PosX,
        PosY,
        PosZ,

        // Integers
        RotX,
        RotY,
        RotZ,

        // Booleans
        Jump,
        Shoot,
        Aim,
        Run,
        Crouch,

        // Floats
        VelocityX,
        VelocityY,
        VelocityZ,

        // ID of picked up weapon
        PickUpWeapon,
        // ID of dropped weapon
        DropWeapon,
        // ID of weapon switched to
        SwitchWeapon,

        // Value is ID of the player who got shot
        ShotPlayer,
        // Value is ID of the player who got the kill
        KilledBy,
        // Value is the amount of health the player has left
        Health,

        NpcId,
        // Npc type
        NpcType,
        // PlayerId of the player who are targeted by the NPC
        NpcTarget
    }
}
