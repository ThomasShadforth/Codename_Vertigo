using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool onGround;
    private float friction;

    private void OnCollisionEnter2D(Collision2D other)
    {
        EvaluateCollision(other);
        GetFriction(other);
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        EvaluateCollision(other);
        GetFriction(other);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
        friction = 0f;
    }

    void EvaluateCollision(Collision2D other)
    {
        for(int i = 0; i < other.contactCount; i++)
        {
            Vector2 normal = other.GetContact(i).normal;
            onGround |= normal.y >= .9f;
        }
    }

    void GetFriction(Collision2D other)
    {
        PhysicsMaterial2D material = other.rigidbody.sharedMaterial;

        friction = 0;

        if(material != null)
        {
            friction = material.friction;
        }
    }

    public bool GetGround()
    {
        return onGround;
    }

    public float GetFrictionVal()
    {
        return friction;
    }
}
