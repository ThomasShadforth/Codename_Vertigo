using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericInputController : ScriptableObject
{
    public abstract float GetMoveInput(float moveDir = 0);
    public abstract bool GetJumpInput();
    public abstract bool GetJumpHoldInput();
    public abstract bool GetAttackInput();
}
