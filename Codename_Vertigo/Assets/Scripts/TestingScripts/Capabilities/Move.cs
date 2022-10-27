using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //The controller this receives input from (AI does this automatically, player's is dependent on input)
    [SerializeField] private GenericInputController input = null;
    //Speed at which the player moves
    [SerializeField, Range(0, 100f)] private float maxSpeed = 4f;
    //Rate at which speed changes on the ground and in the air (Lower = slower, feels heavier. Higher = faster, more responsive)
    [SerializeField, Range(0, 100f)] private float maxAccel = 35f;
    [SerializeField, Range(0, 100f)] private float maxAirAccel = 25f;

    public float xDirection { get; private set; }
    public bool _isKnocked { get; set; }

    //Direction the object is moving
    Vector2 direction;
    //The velocity the object is moving towards
    Vector2 desiredVelocity;
    //The current velocity of the object
    Vector2 velocity;
    //Attached rigidbody
    Rigidbody2D rb2d;
    //Attached ground check script
    GroundCheck ground;
    //Attached Collision Data Check script
    CollisionDataCheck _collisionDataCheck;
    Jump _jump;

    //max speed change during the frame, if any
    float maxSpeedChange;
    //Current acceleration, if any
    float acceleration;
    //If the object is currently grounded (Used for checking what acceleration is used, friction to apply (if applicable), etc.
    bool onGround;
    [SerializeField] float _prevXScale = 1;
    [SerializeField] int _xDirect;
    [SerializeField] bool _facingRight = true;

    private void Awake()
    {
        //Get the attached components
        rb2d = GetComponent<Rigidbody2D>();
        ground = GetComponent<GroundCheck>();
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
        _jump = GetComponent<Jump>();
    }

    private void Update()
    {
        if (_isKnocked || Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            if(Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
            {
                rb2d.velocity = Vector2.zero;
            }
            return;
        }
        //Get the current direction based on the movement input
        if (direction.x != 0) {
            _prevXScale = direction.x; 
        }
        direction.x = input.GetMoveInput(xDirection);

        if(direction.x == 1)
        {
            _xDirect = 1;
        } else if(direction.x == -1)
        {
            _xDirect = -1;
        }

        
        if (direction.x != _prevXScale && _prevXScale != 0 && direction.x != 0)
        {
            if(direction.x > _prevXScale && !_facingRight)
            {
                ChangeXScale();
            } else if(direction.x < _prevXScale && _facingRight)
            {
                ChangeXScale();
            }
            //ChangeXScale();
        }
        

        desiredVelocity = new Vector2(direction.x, 0) * Mathf.Max(maxSpeed - _collisionDataCheck._friction, 0f);
    }

    private void FixedUpdate()
    {
        if (_isKnocked || Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            return;
        }

        onGround = _collisionDataCheck._onGround;
        velocity = rb2d.velocity;
        
        //AdjustVelocity();
        acceleration = onGround ? maxAccel : maxAirAccel;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);


        rb2d.velocity = velocity;
        
    }

    void ChangeXScale()
    {
        _facingRight = !_facingRight;
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
    }

    public int GetXDirect()
    {
        return _xDirect;
    }

    Vector2 ProjectOnContact(Vector2 vector)
    {
        //Subtracts the collision's contact normal scaled by the Dot product of the velocity and contact normal from the original velocity
        //This is used in adjust the velocity of the object when moving up and down slopes
        return vector - _collisionDataCheck._contactNormal * Vector2.Dot(vector, _collisionDataCheck._contactNormal);
    }

    //Adjusts the velocity based on movement on slopes
    void AdjustVelocity()
    {
        Vector2 xAxis = ProjectOnContact(Vector2.right).normalized;
        float currentX = Vector2.Dot(velocity, xAxis);

        acceleration = onGround ? maxAccel : maxAirAccel;
        maxSpeedChange = acceleration * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        velocity += xAxis * (newX - currentX);
    }

    public void SetXDirection(float direction)
    {
        xDirection = direction;
    }
}
