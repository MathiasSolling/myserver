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

        // 1: Object id (Id of player, zombie etc.)
        ObjectId,
        // 2: Object Type (Player, zombie etc.)
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

        // ID of active weapon
        ActiveWeapon,
        // ID of picked up weapon
        PickUpWeapon, // 20
        // ID of dropped weapon
        DropWeapon, 

        // Value is ID of the player who got shot (Client -> Server)
        ShotPlayer,
        // Value is ID of the player who got the kill (Server -> Client)
        KilledBy,
        // Value is the amount of health the player has left (Server -> Client)
        Health,
        // Boolean (Server -> Client)
        Respawn,

        // Typically used if object only has 1 attack type (Server -> Client)
        Attack,

        // Value is amount of kills players have (Server -> Client)
        Kills
    }
}
