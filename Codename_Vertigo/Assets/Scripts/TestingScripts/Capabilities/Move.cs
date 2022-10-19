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

    //max speed change during the frame, if any
    float maxSpeedChange;
    //Current acceleration, if any
    float acceleration;
    //If the object is currently grounded (Used for checking what acceleration is used, friction to apply (if applicable), etc.
    bool onGround;
    float _prevXScale = 1;
    [SerializeField] bool _facingRight = true;

    private void Awake()
    {
        //Get the attached components
        rb2d = GetComponent<Rigidbody2D>();
        ground = GetComponent<GroundCheck>();
    }

    private void Update()
    {

        //Get the current direction based on the movement input
        if (direction.x != 0) {
            _prevXScale = direction.x; 
        }
        direction.x = input.GetMoveInput();

        
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
        

        desiredVelocity = new Vector2(direction.x, 0) * Mathf.Max(maxSpeed - ground.GetFrictionVal(), 0f);
    }

    private void FixedUpdate()
    {
        onGround = ground.GetGround();
        velocity = rb2d.velocity;

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
}
