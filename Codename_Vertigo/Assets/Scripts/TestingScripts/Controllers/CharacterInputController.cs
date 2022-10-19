﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInputController", menuName = "InputController/CharacterInputController")]
public class CharacterInputController : GenericInputController
{
    public override bool GetJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public override float GetMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override bool GetJumpHoldInput()
    {
        return Input.GetButton("Jump");
    }

    public override bool GetAttackInput()
    {
        throw new System.NotImplementedException();
    }
}