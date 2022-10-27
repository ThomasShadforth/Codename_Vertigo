using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIInputController", menuName = "InputController/AIInputController")]
public class AIInputController : GenericInputController
{
    public float moveDirection;
    public bool canJump;
    public bool willJump;

    public override bool GetJumpInput()
    {
        return true;
    }

    public override float GetMoveInput(float moveDir = 0)
    {
        return moveDir;
    }

    public override bool GetJumpHoldInput()
    {
        return false;
    }

    public override bool GetAttackInput()
    {
        return true;
    }

    public bool Patrol(Transform AITransform, Transform patrolPointTransform, float minDistFromPoint)
    {

        float distance = Mathf.Abs(AITransform.position.x) - Mathf.Abs(patrolPointTransform.position.x);
        distance = Mathf.Abs(distance);

        if(distance <= minDistFromPoint)
        {
            
            return true;
        }

        return false;
    }

    public bool Chase(Transform AITransform, Transform targetPosition, float attackDist)
    {
        
        float distance = Mathf.Abs(AITransform.position.x) - Mathf.Abs(targetPosition.position.x);
        distance = Mathf.Abs(distance);

        if(distance <= attackDist)
        {
            //moveDirection = 0;
            return true;
        }

        return false;
    }

    IEnumerator AIWait()
    {
        yield return new WaitForSeconds(10000f);
    }
}
