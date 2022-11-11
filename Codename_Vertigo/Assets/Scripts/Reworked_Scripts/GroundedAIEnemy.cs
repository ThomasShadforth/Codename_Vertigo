using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedAIEnemy : ReworkedBaseAI
{
    // Start is called before the first frame update
    
    Vector2 _rayDir = Vector2.down;

    bool _facingRight = true;
    bool _shouldMaintainHeight = true;
    public bool grounded { get; private set; }

    [Header("Additional Config Values:")]
    [SerializeField] LayerMask _whatIsGround;

    [Header("Height Spring")]
    [SerializeField] float _rideHeight = 1.5f;
    [SerializeField] float _rayToGroundLength = 3f;
    [SerializeField] float _rideSpringStrength = 50f;
    [SerializeField] float _rideSpringDamp = 5f;

    private void FixedUpdate()
    {

        (bool rayHitGround, RaycastHit2D hit) = RaycastToGround();

        grounded = CheckGrounded(rayHitGround, hit);

        //Debug.Log(grounded);

        if (rayHitGround && _shouldMaintainHeight)
        {
            MaintainHeight(hit);
        }

        if (_waiting || isKnocked || _isAttacking || Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if (!isKnocked)
            {
                _rb2d.velocity = Vector2.zero;
            }

            return;
        }

        _velocity = _rb2d.velocity;

        if (currentState == EnemyAIFSM.idle)
        {
            
        } else if(currentState == EnemyAIFSM.patrol)
        {
            Patrol();
        } else if(currentState == EnemyAIFSM.chase)
        {
            
            Chase();
        }

        EnemyJump();
        AnimateEnemy();

        _rb2d.velocity = _velocity;

        //Section: AI Behaviour based on current state
    }

    protected override void CheckState()
    {
        if (_waiting || isKnocked || _isAttacking || Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if (!isKnocked)
            {
                _rb2d.velocity = Vector2.zero;
            }

            return;
        }

        float prevDir = direction;

        float dist = Vector2.Distance(transform.position, _target.transform.position);

        if(dist <= chaseDist && currentState != EnemyAIFSM.chase)
        {
            currentState = EnemyAIFSM.chase;
        } else if(dist > chaseDist && currentState == EnemyAIFSM.chase)
        {
            currentState = _defaultState;
        }

        //
        if(currentState == EnemyAIFSM.patrol || currentState == EnemyAIFSM.chase)
        {
            
            if(currentState == EnemyAIFSM.patrol)
            {
                direction = _patrolPoints[_currentPatrolIndex].position.x > transform.position.x ? 1f : -1f;

                _desiredVelocity = new Vector2(direction, 0) * Mathf.Max(_patrolMoveSpeed, 0);
            } else if(currentState == EnemyAIFSM.chase)
            {
                direction = _target.transform.position.x > transform.position.x ? 1f : -1f;
                _desiredVelocity = new Vector2(direction, 0) * Mathf.Max(_chaseMoveSpeed, 0);
            }
        }

        if(direction != prevDir)
        {
            if(direction > prevDir && !_facingRight)
            {
                _facingRight = true;
                FlipXScale();
            } else if (direction < prevDir && _facingRight)
            {
                _facingRight = false;
                FlipXScale();
            }
        }
    }

    #region State Behaviours

    protected override void Patrol()
    {
        _maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        //Place rest of patrol logic here
        if(Vector2.Distance(transform.position, _patrolPoints[_currentPatrolIndex].position) <= minDistToPatrol)
        {
            //Set waiting variable to true

            _currentPatrolIndex++;
            if(_currentPatrolIndex >= _patrolPoints.Length)
            {
                _currentPatrolIndex = 0;
            }
            _waiting = true;
            _velocity.x = 0f;
            StartCoroutine(ResetWait());
            
        }
    }

    protected override void Chase()
    {
        _maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        //Add checks for attack/jump here

        float xDist = Mathf.Abs(transform.position.x - _target.transform.position.x);

        float yDist = _target.transform.position.y - transform.position.y;
        
        //Debug.Log(yDist);

        if (canJump)
        {
            if (xDist <= attackDist)
            {
                if (yDist > minDistToJump)
                {

                    if (!_isJumping)
                    {
                        _desiredJump = true;
                    }
                }
            }
        }

        if(Vector2.Distance(transform.position, _target.position) <= attackDist)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _rb2d.velocity = Vector2.zero;
                _velocity = Vector2.zero;
                _animator.Play("Attack");
            }
        }
    }

    #endregion

    #region Jump/Wall Jump

    void EnemyJump()
    {
        if (grounded)
        {
            _coyoteCounter = coyoteTime;
            _isJumping = false;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        //Use desired jump maybe
        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = jumpBuffer;
        } else if(!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if(_jumpBufferCounter > 0)
        {
            EnemyJumpAction();
        }

        if(_velocity.y > 0)
        {
            _rb2d.gravityScale = jumpGravity;
        }

        if(_velocity.y < 0)
        {
            _rb2d.gravityScale = fallGravity;
        }

        if (grounded)
        {
            _rb2d.gravityScale = 1f;
        }

    }

    void EnemyJumpAction()
    {
        if(_coyoteCounter > 0 || (_isJumping && _jumpPhase < maxJumpCount))
        {
            _coyoteCounter = 0;

            float jumpSpeed = Mathf.Sqrt(-2 * Physics2D.gravity.y * jumpHeight);

            _isJumping = true;

            if(_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0);
            }

            _velocity.y += jumpSpeed;
        }
    }

    #endregion

    #region Ground Checks

    bool CheckGrounded(bool rayHitGround, RaycastHit2D hit)
    {
        bool grounded = false;

        if (rayHitGround)
        {
            grounded = hit.distance < _rideHeight * 1.3f;
        }
        else
        {
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

        Rigidbody2D _hitrb2d = hit.rigidbody;

        if(_hitrb2d != null)
        {
            //otherVel = _hitrb2d.velocity;
        }

        float rayDirVelocity = Vector2.Dot(_rayDir, vel);
        float otherDirVelocity = Vector2.Dot(_rayDir, otherVel);

        float relativeVelocity = rayDirVelocity - otherDirVelocity;
        float currHeight = hit.distance - _rideHeight;
        float springForce = (currHeight * _rideSpringStrength) - (relativeVelocity * _rideSpringDamp);
        Vector2 maintainHeightForce = -_gravForce + springForce * Vector2.down;

        _rb2d.AddForce(maintainHeightForce);

        if(_hitrb2d != null)
        {
            //Apply force to other rigidbody if necessary;
        }
    }

    #endregion

    #region Hit Checks

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ReworkedPlayerController player = other.GetComponent<ReworkedPlayerController>();
            player.Damage(20, transform);
        }
    }

    #endregion

    #region Misc

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, chaseDist);
    }

    IEnumerator ResetWait()
    {
        yield return new WaitForSeconds(1f);
        _waiting = false;
    }

    public void AnimateEnemy()
    {
        if(_velocity.x != 0)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }
    }

    public void FlipXScale()
    {
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
    }

    public void ResetAnimation()
    {
        _isAttacking = false;
        _animator.Play("IDLE");
    }

    #endregion
}
