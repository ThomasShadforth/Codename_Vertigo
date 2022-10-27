using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpSlide : MonoBehaviour
{
    public bool wallJumping { get; private set; }
    [SerializeField] [Range(0.1f, 5f)] float _wallSlideMaxSpeed = 2f;
    [SerializeField] Vector2 _wallJumpClimb = new Vector2(4f, 12f);
    [SerializeField] Vector2 _wallJumpBounce = new Vector2(10.7f, 10f);
    [SerializeField] Vector2 _wallJumpLeap = new Vector2(14f, 12f);
    [SerializeField] GenericInputController input = null;

    CollisionDataCheck _collisionDataCheck;
    Rigidbody2D _rb2d;

    Vector2 _velocity;
    public bool _onWall { get; private set; }
    bool _onGround, _desiredJump;
    float _wallDirectionX;

    [SerializeField] ParticleSystem _wallSlideDust;

    // Start is called before the first frame update
    void Awake()
    {
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
        _rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_onWall && !_onGround)
        {
            _desiredJump |= input.GetJumpInput();
        }
    }

    private void FixedUpdate()
    {
        _velocity = _rb2d.velocity;
        _onWall = _collisionDataCheck._onWall;
        _onGround = _collisionDataCheck._onGround;
        _wallDirectionX = _collisionDataCheck._contactNormal.x;

        //Wall Sliding
        if (_onWall)
        {
            if(_velocity.y < -_wallSlideMaxSpeed)
            {
                _velocity.y = -_wallSlideMaxSpeed;
                if (!_wallSlideDust.isPlaying)
                {
                    _wallSlideDust.Play();
                }
            }
        }

        //Wall Jumping
        if((_onWall && _velocity.x == 0) || _onGround)
        {
            wallJumping = false;
        }

        if (_desiredJump)
        {
            if(-_wallDirectionX == input.GetMoveInput())
            {
                _velocity = new Vector2(_wallJumpClimb.x * _wallDirectionX, _wallJumpClimb.y);
                wallJumping = true;
                _desiredJump = false;
            } else if(input.GetMoveInput() == 0)
            {
                _velocity = new Vector2(_wallJumpBounce.x * _wallDirectionX, _wallJumpBounce.y);
                wallJumping = true;
                _desiredJump = false;
            }
            else
            {
                _velocity = new Vector2(_wallJumpLeap.x * _wallDirectionX, _wallJumpLeap.y);
                wallJumping = true;
                _desiredJump = false;
            }
        }

        _rb2d.velocity = _velocity;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _collisionDataCheck.EvaluateCollision(other);

        if(_collisionDataCheck._onWall && !_collisionDataCheck._onGround && wallJumping)
        {
            _rb2d.velocity = Vector2.zero;
        }
    }
}
