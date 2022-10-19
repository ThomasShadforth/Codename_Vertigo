using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private GenericInputController input = null;
    [SerializeField, Range(0, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0, 100f)] private float maxAccel = 35f;
    [SerializeField, Range(0, 100f)] private float maxAirAccel = 25f;

    Vector2 direction;
    Vector2 desiredVelocity;
    Vector2 velocity;
    Rigidbody2D rb2d;
    GroundCheck ground;

    float maxSpeedChange;
    float acceleration;
    bool onGround;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ground = GetComponent<GroundCheck>();
    }

    private void Update()
    {
        direction.x = input.GetMoveInput();
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
}
