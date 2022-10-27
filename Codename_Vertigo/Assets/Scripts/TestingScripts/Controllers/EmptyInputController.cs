using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EmptyInputController", menuName = "InputController/EmptyInputController")]
public class EmptyInputController : GenericInputController
{
    public override bool GetAttackInput()
    {
        return false;
    }

    public override bool GetJumpHoldInput()
    {
        return false;
    }

    public override bool GetJumpInput()
    {
        return false;
    }

    public override float GetMoveInput(float moveDir = 0)
    {
        return 0;
    }
}
