using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] GenericInputController input = null;
    [SerializeField, Range(0f, 10f)] float _jumpHeight = 5f;
    [SerializeField, Range(0, 10f)] int _maxAirJump = 0;
    [SerializeField, Range(0, 10f)] float _downwardAirMultiplier = 3f;
    [SerializeField, Range(0, 10f)] float _upwardAirMultiplier = 1.7f;
    [SerializeField, Range(0, 1f)] float _wallSlideMultiplier = .7f;
    [SerializeField, Range(0, 0.9f)] float _coyoteTime = .2f;
    [SerializeField, Range(0, 0.5f)] float _jumpBufferTime = .2f;

    Rigidbody2D _rb2d;
    GroundCheck _ground;
    WallJumpCheck _wallJump;
    Vector2 _velocity;

    int _jumpPhase;
    float _defaultGravityScale, _coyoteCounter, _jumpBufferCounter;

    bool _desiredJump;
    bool _onGround;
    bool _wallSliding;
    bool _isJumping;

    // Start is called before the first frame update
    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _ground = GetComponent<GroundCheck>();
        _wallJump = GetComponent<WallJumpCheck>();
        _defaultGravityScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        _desiredJump |= input.GetJumpInput();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetGround();
        _velocity = _rb2d.velocity;

        if (_onGround)
        {
            Vector2 velo = _rb2d.velocity;
            velo.y = 0f;
            _rb2d.velocity = velo;
        }

        if(_onGround && _rb2d.velocity.y == 0)
        {
            _jumpPhase = 0;
            _coyoteCounter = _coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        } else if(!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if(_jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if (input.GetJumpHoldInput() && _rb2d.velocity.y > 0)
        {
            _rb2d.gravityScale = _upwardAirMultiplier;
        }

        if(!input.GetJumpHoldInput() || _rb2d.velocity.y < 0)
        {
            if (_wallJump.CheckForWall((int)input.GetMoveInput()))
            {
                //_rb2d.gravityScale = _wallSlideMultiplier;
                _velocity = new Vector2(_velocity.x, -.3f);
            }
            else
            {
                _rb2d.gravityScale = _downwardAirMultiplier;
            }
        }

        if(_rb2d.velocity.y == 0)
        {
            _rb2d.gravityScale = _defaultGravityScale;
        }

        _rb2d.velocity = _velocity;

        
    }

    //To do: Rework this version of the jump action to account for wall jumps
    void JumpAction()
    {
        
        if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJump && _isJumping))
        {
            if (_isJumping)
            {
                _jumpPhase++;
            }
            Debug.Log("JUMPING");
            _coyoteCounter = 0f;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);

            

            _isJumping = true;

            if(_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }
            _velocity.y += jumpSpeed;

        }
    }
}
