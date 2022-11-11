using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedPlayerController : MonoBehaviour, IDamageInterface, IDataPersistence
{
    Rigidbody2D _rb2d;
    Vector2 gravForce;
    Vector2 _rayDir = Vector2.down;

    [SerializeField] bool _facingRight = true;
    bool _shouldMaintainHeight = true;
    public bool grounded { get; private set; }
    float _moveInput;
    bool _desiredJump;
    bool _isJumping;
    bool _isKnocked;
    bool _isAttacking;
    public int _xDirect;

    [Header("Config Values:")]
    [SerializeField] LayerMask _whatIsGround;

    [Header("Health:")]
    [SerializeField] int _playerMaxHealth;
    [SerializeField] HealthBar _healthBar;

    [Header("Movement:")]
    [SerializeField] float _maxSpeed = 4f;
    [SerializeField] float _acceleration;
    Vector2 _desiredVelocity;
    Vector2 _velocity;
    float _maxSpeedChange;
    float _prevMoveInput;

    [Header("Height Spring:")]
    [SerializeField] float _rideHeight = 1.5f;
    [SerializeField] float _rayToGroundLength = 3f;
    public float _rideSpringStr = 50f;
    [SerializeField] float _rideSpringDamp = 5f;

    [Header("Jump Values:")]
    [SerializeField] float _jumpHeight = 4f;
    [SerializeField] float _jumpGravity = 1.7f;
    [SerializeField] float _fallGravity = 3f;
    [SerializeField] float _jumpBuffer = .15f;
    [SerializeField] float _coyoteTime = .25f;
    [SerializeField] int maxJumps;
    float _jumpBufferCount;
    float _coyoteCounter;
    float _jumpPhase;
    bool _prevGrounded = false;

    [Header("Wall Jump:")]
    [SerializeField] float _wallCheckDist;
    [SerializeField] float _wallSlideVelocity;
    [SerializeField] Vector2 _wallClimb;
    [SerializeField] Vector2 _wallBounce;
    [SerializeField] Vector2 _wallLeap;
    [SerializeField] LayerMask _wallCheckLayer;

    [Header("Knockback Values:")]
    public Vector2 knockForce;
    public float knockTimer;

    [Header("Particle Effects:")]
    [SerializeField] ParticleSystem _jumpEffect;
    [SerializeField] ParticleSystem _wallEffect;

    public bool _onWall { get; private set; }
    bool _wallJumping;

    HealthSystem _healthSystem;
    PlayerCombat _combat;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _healthSystem = new HealthSystem(_playerMaxHealth);
        _healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        _combat = GetComponent<PlayerCombat>();
        gravForce = Physics2D.gravity * _rb2d.mass;

        Debug.Log(_rideHeight * 1.3);
    }

    private void Update()
    {
        if (!Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (_combat != null)
                {
                    _combat.SetAttack();
                }
            }
        }

        if (_isKnocked || _combat._isAttacking || Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if (!_isKnocked)
            {
                _rb2d.velocity = Vector2.zero;
            }

            return;
        }

        _desiredJump |= Input.GetButtonDown("Jump");

        

        _desiredVelocity = new Vector2(_moveInput, 0) * Mathf.Max(_maxSpeed, 0);

    }

    private void FixedUpdate()
    {
        (bool rayHitGround, RaycastHit2D hit) = RaycastToGround();

        grounded = CheckGrounded(rayHitGround, hit);

        //Debug.Log(grounded);

        if (grounded)
        {
            //Determine whether to add the check for landing sfx
        }
        else
        {

        }

        if (rayHitGround && _shouldMaintainHeight)
        {
            MaintainHeight(hit);
        }

        if (_isKnocked || _combat._isAttacking || Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if (!_isKnocked)
            {
                _rb2d.velocity = Vector2.zero;
            }

            return;
        }

        _velocity = _rb2d.velocity;

        if (_moveInput != 0  && !_wallJumping)
        {
            _prevMoveInput = _moveInput;
        }

        if(Input.GetAxisRaw("Horizontal") > 0)
        {
            _moveInput = 1f;
        } else if(Input.GetAxisRaw("Horizontal") < 0)
        {
            _moveInput = -1f;
        }
        else
        {
            _moveInput = 0f;
        }
        //_moveInput = Input.GetAxisRaw("Horizontal");

        if (_moveInput != _prevMoveInput && _moveInput != 0)
        {
            
            if (_moveInput > _prevMoveInput && !_facingRight)
            {
                _facingRight = true;
                SwitchXScale();
            }
            else if (_moveInput < _prevMoveInput && _facingRight)
            {
                
                _facingRight = false;
                SwitchXScale();
            }
        }

        //Debug.Log(rayHitGround);

        PlayerMove();
        PlayerWallCheck();
        PlayerJump();

        _rb2d.velocity = _velocity;

    }

    #region Movement

    void PlayerMove()
    {
        _maxSpeedChange = _acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
    }

    #endregion

    #region Jumping / Wall Jumping

    void PlayerJump()
    {
        if (grounded)
        {
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
            _jumpBufferCount = _jumpBuffer;
        }
        else if(!_desiredJump && _jumpBufferCount > 0)
        {
            _jumpBufferCount -= Time.deltaTime;
        }

        if(_jumpBufferCount > 0)
        {
            JumpAction();
        }

        if(Input.GetButton("Jump") && _velocity.y > 0)
        {
            _rb2d.gravityScale = _jumpGravity;
            _shouldMaintainHeight = false;
        }

        if(!Input.GetButton("Jump") || (_velocity.y < 0 && !grounded))
        {
            _rb2d.gravityScale = _fallGravity;
            _shouldMaintainHeight = true;
        }

        if(grounded)
        {
            _rb2d.gravityScale = 1f;
        }
        //Insert checks for velocity/Checks for holding jump input
    }

    void JumpAction()
    {
        //Add check for jump count (though unnecessary here)
        if(_coyoteCounter > 0 || (_isJumping && _jumpPhase < maxJumps))
        {
            
            _coyoteCounter = 0f;
            float jumpSpeed = Mathf.Sqrt(-2 * Physics2D.gravity.y * _jumpHeight);

            _jumpEffect.Play();

            _isJumping = true;

            if(_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }

            _velocity.y += jumpSpeed;
        }
    }

    void PlayerWallCheck()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, transform.right, _wallCheckDist * _xDirect, _wallCheckLayer);
        Debug.DrawRay(transform.position, transform.right * _wallCheckDist * _xDirect, Color.blue);


        if (wallHit && !grounded)
        {
            _onWall = true;

        }
        else
        {
            _onWall = false;
        }

        if (_onWall)
        {
            if (!_wallEffect.isPlaying)
            {
                _wallEffect.Play();
            }

            float dir = wallHit.normal.x;
            dir = Mathf.RoundToInt(dir);
            
            if (_velocity.y < -_wallSlideVelocity)
            {
                _velocity.y = -_wallSlideVelocity;
            }

            if (_desiredJump)
            {
                if(-dir == _moveInput)
                {
                    _velocity = new Vector2(_wallClimb.x * dir, _wallClimb.y);
                    _wallJumping = true;
                    
                    //Wall Climb
                } else if(_moveInput == 0)
                {
                    _velocity = new Vector2(_wallBounce.x * dir, _wallBounce.y);
                    _wallJumping = true;
                   
                    //Wall Bounce
                } else
                {
                    _velocity = new Vector2(_wallLeap.x * dir, _wallLeap.y);
                    _wallJumping = true;
                    
                    //Wall Leap
                }

                _jumpEffect.Play();
                _wallEffect.Stop();

                _onWall = false;
                SwitchXScale();
                _facingRight = !_facingRight;
                _prevMoveInput = -_prevMoveInput;
                StartCoroutine(ResetWallJump());
            }
        }

    }

    #endregion

    #region Checks For Ground

    bool CheckGrounded(bool rayHitGround, RaycastHit2D rayHit)
    {
        
        bool grounded;
        if (rayHitGround)
        {
            if(rayHit.distance > _rideHeight * 1.7f)
            {
                //Debug.Log(rayHit.distance);
            }
            grounded = rayHit.distance < _rideHeight * 1.7f;
            
        }
        else
        {
            //Debug.Log("NO HIT");
            grounded = false;
        }

        return grounded;
    }

    (bool, RaycastHit2D) RaycastToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundLength, _whatIsGround);
        
        bool rayHitGround = Physics2D.Raycast(transform.position, Vector2.down, _rayToGroundLength, _whatIsGround);
        Debug.DrawRay(transform.position, Vector2.down * _rayToGroundLength, Color.red);
        
        return (rayHitGround, hit);
    }

    #endregion

    #region Height Maintenance

    void MaintainHeight(RaycastHit2D hit)
    {
        Vector2 vel = _rb2d.velocity;
        Vector2 otherVel = Vector2.zero;
        Rigidbody2D hitrb2d = hit.rigidbody;

        if(hitrb2d != null)
        {

        }

        float rayDirVelocity = Vector2.Dot(_rayDir, vel);
        float otherDirVelocity = Vector2.Dot(_rayDir, otherVel);

        float relativeVel = rayDirVelocity - otherDirVelocity;
        float currHeight = hit.distance - _rideHeight;
        float springForce = (currHeight * _rideSpringStr) - (relativeVel * _rideSpringDamp);
        Vector2 maintainHeightForce = -gravForce + springForce * Vector2.down;

        _rb2d.AddForce(maintainHeightForce);

        if(hitrb2d != null)
        {
            //Add force to hit rigidbody if applicable
        }
    }

    #endregion

    #region Health Management

    void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        Debug.Log(_healthSystem.GetHealth());

        if (_healthBar != null)
        {
            _healthBar.SetHealthFill(_healthSystem.GetHealthPercent());
        }
    }

    public void Damage(float damage, Transform attackPos)
    {
        //Deal damage, apply knockback
        _healthSystem.Damage((int)damage);

        Vector2 knockDir = transform.position - attackPos.position;
        knockDir = knockDir.normalized;
        knockDir.y = .3f;
        GetComponent<Animator>().Play("Player_Hit");
        StartCoroutine(KnockbackCo(knockDir));

        //Apply the knockback
    }

    IEnumerator KnockbackCo(Vector2 knockDir)
    {
        float knockCounter = knockTimer;

        while(knockCounter > 0)
        {
            if (!_isKnocked)
            {
                _isKnocked = true;
                _isAttacking = false;
                _rb2d.velocity = new Vector2(knockDir.x * knockForce.x, knockDir.y * knockForce.y);
            }

            knockCounter -= Time.deltaTime;
            yield return null;
        }

        _isKnocked = false;
    }

    public void CheckForHealth()
    {

    }

    #endregion

    #region Data Management

    public void SaveData(GameData data)
    {
        data.playerPosition = CheckpointManager.instance.currentCheckpointPos;
    }

    public void LoadData(GameData data)
    {
        transform.position = data.playerPosition;
    }

    #endregion

    #region Misc

    void SwitchXScale()
    {
        _xDirect *= -1;
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
    }

    IEnumerator ResetWallJump()
    {
        yield return new WaitForSeconds(.25f);
        _wallJumping = false;
    }

    #endregion

}
