using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IWallJumpable
{
    public Vector3 offset;
    public Vector3 pos;

    BoxCollider2D collider;

    public Vector2 upperLeftCornerPos;
    public Vector2 upperRightCornerPos;
    Vector2 lowerLeftCornerPos;
    Vector2 lowerRightCornerPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWallJump(CustomPhysics physicsObject)
    {
        physicsObject.onWall = true;
        physicsObject.gravityModifier = .2f;
    }

    public void DisableWallJump(CustomPhysics physicsObject)
    {
        physicsObject.onWall = false;
        physicsObject.gravityModifier = 1f;
    }

    Vector2 SetWallCorner(Vector2 cornerToSet, int sideX, int sideY)
    {
        
        cornerToSet = new Vector2(collider.offset.x + ((collider.size.x / 2) * sideX), collider.offset.y + ((collider.size.y / 2) * sideY));
        cornerToSet = transform.TransformPoint(cornerToSet);
        GameObject newCorner = new GameObject();
        newCorner.transform.position = cornerToSet;
        if(sideX == 1)
        {
            newCorner.name = "Right Corner";
        }
        else
        {
            newCorner.name = "Left Corner";
        }

        
        return cornerToSet;
    }

    public Vector2 GetCorner(string sideToGet)
    {
        if(sideToGet == "UpperLeft")
        {
            return upperLeftCornerPos;
        } else if(sideToGet == "UpperRight")
        {
            return upperRightCornerPos;
        }
        else if(sideToGet == "LowerLeft")
        {
            return lowerLeftCornerPos;
        } else if(sideToGet == "LowerRight")
        {
            return lowerRightCornerPos;
        }
        else
        {
            return Vector2.zero;
        }
        
    }
}
