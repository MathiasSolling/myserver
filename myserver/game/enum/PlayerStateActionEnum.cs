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

        // Object id (Id of player, zombie etc.)
        ObjectId,
        // Object Type (Player, zombie etc.)
        ObjectType,

        PackageSeqNum,

        // Floats
        PosX,
        PosY,
        PosZ,

        // Integers
        RotX,
        RotY,
        RotZ,

        ShootRotationX,

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

        // Value is ID of the player who got shot (Client -> Server)
        ShotPlayer,
        // Value is ID of the player who got the kill (Server -> Client)
        KilledBy, 
        // Value is the amount of health the player has left (Server -> Client)
        Health,

        // Typically used if object only has 1 attack type (Server -> Client)
        Attack
    }
}
