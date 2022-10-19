using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIInputController", menuName = "InputController/AIInputController")]
public class AIInputController : GenericInputController
{
    public override bool GetJumpInput()
    {
        return true;
    }

    public override float GetMoveInput()
    {
        return 1f;
    }

    public override bool GetJumpHoldInput()
    {
        return false;
    }

    public override bool GetAttackInput()
    {
        return true;
    }
}
