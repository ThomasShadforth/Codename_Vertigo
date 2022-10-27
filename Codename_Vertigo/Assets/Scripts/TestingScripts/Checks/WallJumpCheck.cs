using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpCheck : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] float detectWallRange = 3f;
    [SerializeField] LayerMask wallLayer;
    private bool onWall;
    private bool wallSliding;

    public bool CheckForWall(int xDirect)
    {
        
        Vector2 rayDirection = Vector2.right + new Vector2(detectWallRange - 1, 0);
        rayDirection.x = rayDirection.x * xDirect;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, rayDirection, detectWallRange, wallLayer);

        Debug.DrawRay(transform.position, rayDirection);

        if (wallHit && Input.GetAxisRaw("Horizontal") != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        return wallSliding;
    }

    public bool GetWallSliding()
    {
        return wallSliding;
    }

    
}
