using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : CustomPhysics
{
    
    public bool stuckToWall;
    public Vector2 directionOfSpear;

    public Vector2[] arcPositions;
    // Start is called before the first frame update

    private void Awake()
    {
        
        velocity.y = directionOfSpear.y;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    
    protected override void ComputeVelocity()
    {
        Vector2 spearMove = Vector2.zero;

        spearMove.x = directionOfSpear.x;

        targetVelocity = spearMove * 4;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


}
