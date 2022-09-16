using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedPhysicsObject : MonoBehaviour
{
    /// <summary>
    /// Distance to ground where player is grounded
    /// </summary>
    [SerializeField] protected float groundDist = .01f;
    /// <summary>
    /// Distance to ground at which player is considered standing on an object
    /// </summary>
    [SerializeField] protected float standingDist = .1f;
    /// <summary>
    /// Used as the distance of player to the ground
    /// </summary>
    [SerializeField] protected float groundCheckDist = 5f;


    [SerializeField] [Range(0, 90)] private float maxWalkAngle = 60f;
    [SerializeField] protected float walkSpeed;
    /// <summary>
    /// Rate at which momentum is decreased when walking into objects at an angle
    /// </summary>
    [SerializeField] protected float anglePower;

    /// <summary>
    /// Speed at which player is pushed out of overlapped objects
    /// </summary>
    [SerializeField] protected float pushSpeed = 1.0f;

    [SerializeField] protected float verticalSnapDown = .2f;

    [Header("Jump Config")]
    [SerializeField] protected float jumpVelocity = 5.0f;

    [SerializeField] [Range(0, 1)] protected float jumpAngleWeightFactor;

    [SerializeField] protected float leewayTime = .25f;

    protected Vector2 inputMove;
    protected bool jumpPressed;
    protected Vector2 targetVelocity;
    protected float distToGround;
    protected bool onGround;
    protected float angle;
    protected Vector2 surfaceNormal;
    protected Vector2 groundHitPos;
    protected GameObject floor;

    protected Rigidbody2D rb2d;

    protected bool previousGrounded;
    protected bool startGrounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
