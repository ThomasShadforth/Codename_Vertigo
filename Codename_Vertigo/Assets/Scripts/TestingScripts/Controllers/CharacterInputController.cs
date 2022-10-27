using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInputController", menuName = "InputController/CharacterInputController")]
public class CharacterInputController : GenericInputController
{
    public override bool GetJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public override float GetMoveInput(float moveDir = 0)
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override bool GetJumpHoldInput()
    {
        return Input.GetButton("Jump");
    }

    public override bool GetAttackInput()
    {
        return Input.GetButtonDown("Fire1");
    }

    public bool GetSpearInput()
    {
        return Input.GetButton("Fire2");
    }

    public bool GetSpearInputDown()
    {
        return Input.GetButtonDown("Fire2");
    }

    public bool GetSpearInputUp()
    {
        return Input.GetButtonUp("Fire2");
    }


}
