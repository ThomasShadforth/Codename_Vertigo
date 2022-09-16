using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallJumpable
{
    void SetWallJump(CustomPhysics physicsObject);
    void DisableWallJump(CustomPhysics physicsObject);
}
