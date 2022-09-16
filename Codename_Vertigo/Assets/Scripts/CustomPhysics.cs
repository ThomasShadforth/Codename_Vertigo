using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;
    public bool canWallJump;
    public bool onWall;
    public bool wallSliding;

    protected Vector2 targetVelocity;
    protected bool grounded = false;
    protected Vector2 groundNormal;

    [SerializeField]
    protected Vector3 transformAtCollision;
    protected bool collideWall;

    protected Rigidbody2D rb2d;
    //General velocity vector, Used for custom physics
    protected Vector2 velocity;
    
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.0002f;
    protected const float shellRadius = 0.008f;

    public bool onPlatform;
    public Vector2 platformPos;

    [SerializeField] protected bool isDying;

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying)
        {
            return;
        }

        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        if (isDying)
        {
            return;
        }
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * deltaPosition.y;

        Movement(move, true);

        transformAtCollision = rb2d.position;

        

    }

    void Movement(Vector2 move, bool yMovement)
    {
        
        float distance = move.magnitude;
        

        if (distance > minMoveDistance)
        {
            //rigidbody2d cast will cast the collider's shape in a specific direction.
            //This will then check whether or not this overlaps another collider in the next frame
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            hitBufferList.Clear();

            gravityModifier = 1f;
            onWall = false;

            for(int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }


            for (int i = 0; i < hitBufferList.Count; i++)
            {
                

                

                //Check the normal of the objects to determine the angle of the collision
                Vector2 currentNormal = hitBufferList[i].normal;
                //Debug.Log(currentNormal);

                if (currentNormal.x == 1 || currentNormal.x == -1)
                {
                    
                    collideWall = true;

                }
                else
                {
                    collideWall = false;
                }

                if(currentNormal.y > minGroundNormalY)
                {
                    
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);

                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

        }
        
        rb2d.position = rb2d.position + move.normalized * distance;
        
    }

    public virtual void SetDying()
    {
        isDying = true;
    }
}
