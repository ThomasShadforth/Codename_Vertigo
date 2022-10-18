using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : CustomPhysics, IDataPersistence, IDamageInterface, IKnockbackInterface
{
    public float maxSpeed = 7f;
    public float jumpHeight = 7f;
    public KeyCode lastKeyPressed;
    public float keyPressInterval;
    public float keyTimer;
    float keyTime;
    public float dashDistance;
    public float dashTime;
    public float minDashTimer = .2f;
    float minDashTime;

    public float knockbackTime;
    public float knockForce;

    public bool isDashing;
    public bool wallJumping;

    public float wallClingRange;
    public int xDirect;
    public LayerMask wallJumpLayer;

    public bool facingRight;
    bool isJumping;
    bool isFalling;

    public bool isAttacking;

    [SerializeField] AudioSource audioSource;

    Animator animator;
    
    HealthSystem healthSystem;
    HealthBar playerHealthBar;

    PlayerCombat combatSystem;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        combatSystem = GetComponent<PlayerCombat>();
        xDirect = 1;
        facingRight = true;
        healthSystem = new HealthSystem(100);
        playerHealthBar = GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>();
        
        if(playerHealthBar != null)
        {
            Debug.Log("HEALTH FOUND");
        }

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    // Update is called once per frame
    protected override void ComputeVelocity()
    {
        if (isKnocked || GameManager.instance.isPaused)
        {
            return;
        }

        if (Dialogue_Manager.instance.dialogueIsPlaying)
        {
            animator.speed = 0f;
            return;
        } else
        {
            if(animator.speed == 0f)
            {
                animator.speed = 1f;
            }
        }

        if (grounded)
        {
            isFalling = false;
            
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            healthSystem.Heal(10);
        } else if (Input.GetKeyDown(KeyCode.P))
        {
            healthSystem.Damage(10);
        }

        if (!isDashing)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (keyTime > 0 && lastKeyPressed == KeyCode.A)
                {
                    
                    StartCoroutine(PlayerDash(-1));
                    //Dash
                }
                else
                {
                    keyTime = keyTimer;
                }

                lastKeyPressed = KeyCode.A;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (keyTime > 0 && lastKeyPressed == KeyCode.D)
                {
                    //Dash
                    
                    StartCoroutine(PlayerDash(1));
                }
                else
                {
                    keyTime = keyTimer;
                }

                lastKeyPressed = KeyCode.D;
            }

            
        }
        if (isDashing)
        {
            if (((Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) && minDashTime > minDashTimer) || ((collideWall || onWall) && minDashTime > minDashTimer))
            {
                StopDash();
            }

            minDashTime += Time.deltaTime;

            
        }

        if(lastKeyPressed != KeyCode.None)
        {
            keyTime -= Time.deltaTime;
            if(keyTime <= 0)
            {
                lastKeyPressed = KeyCode.None;
            }
        }

        if (Input.GetMouseButtonDown(0) && !GetComponent<SpearThrow>().GetSpearThrown())
        {
            combatSystem.Attack();
        }
        

        if (!isDashing || !wallJumping)
        {
            Vector2 move = Vector2.zero;

            float prevMoveX = move.x;

            move.x = Input.GetAxisRaw("Horizontal");

            if(move.x != prevMoveX)
            {
                if (move.x > prevMoveX && !facingRight)
                {
                    FlipPlayerSprite();
                } else if(move.x < prevMoveX && facingRight)
                {
                    FlipPlayerSprite();
                }
            }

            if(move.x == 1)
            {
                xDirect = 1;
            } else if(move.x == -1)
            {
                xDirect = -1;
            }

            targetVelocity = move * maxSpeed;
        }

        if (Input.GetButtonDown("Jump"))
        {
            
            if (grounded)
            {
                dustEffect.Play();
                velocity.y = jumpHeight;
            }
            else if (wallSliding)
            {
                //Debug.Log("WALL JUMP!");
                StartCoroutine(WallJump());
            }
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * .5f;
            }
        }


        if (!wallJumping)
        {
            CheckForWallCling();
        }

        AnimateCharacter();

    }

    void CreateDust()
    {
        
        dustEffect.Play();
    }
    

    #region Wall Jumping/Sliding
    public void CheckForWallCling()
    {
        Vector2 directionOfRay = Vector2.right + new Vector2(wallClingRange - 1, 0);
        directionOfRay.x = directionOfRay.x * xDirect;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionOfRay, wallClingRange, wallJumpLayer);

        Debug.DrawRay(transform.position, directionOfRay, Color.red);

        onWall = Physics2D.OverlapCircle(new Vector2(transform.position.x + (.3f * xDirect), transform.position.y), .1f, wallJumpLayer);

        if (onWall && Input.GetAxisRaw("Horizontal") != 0)
        {
            wallSliding = true;

            if (!wallDustEffect.gameObject.activeInHierarchy)
            {

                wallDustEffect.gameObject.SetActive(true);
            }
            gravityModifier = 1.3f;
            
            velocity.y = velocity.y * .5f;
        }
        else
        {
            wallSliding = false;
            gravityModifier = 1f;
        }
    }

    public IEnumerator WallJump()
    {
        wallDustEffect.gameObject.SetActive(false);
        xDirect *= -1;
        velocity.y = jumpHeight * .9f;
        dustEffect.Play();
        rb2d.velocity = new Vector2(-Input.GetAxisRaw("Horizontal") * 15, rb2d.velocity.y);

        wallJumping = true;

        while (rb2d.velocity.x != 0)
        {
            rb2d.velocity = Vector2.MoveTowards(rb2d.velocity, Vector2.zero, 80f * Time.deltaTime);
            yield return null;
        }

        rb2d.velocity = Vector2.zero;
        wallJumping = false;

    }

    #endregion

    #region Sound Effects

    public void PlaySoundEffect()
    {
        //For now, simply play the effect attached to the player's audio source
        audioSource.Play();
    }

    #endregion

    #region Dashing
    public IEnumerator PlayerDash(float multiplier)
    {
        isDashing = true;
        rb2d.velocity = new Vector2(dashDistance * multiplier, velocity.y);
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb2d.velocity = Vector2.zero;
    }

    void StopDash()
    {
        isDashing = false;
        rb2d.velocity = Vector2.zero;

        if (collideWall || onWall)
        {
            rb2d.position = transformAtCollision;
        }
    }

    #endregion

    #region Animating

    void AnimateCharacter()
    {
        if (isAttacking)
        {
            return;
        }
        ///Player movement animations
        ///Responsible for run movements
        if(targetVelocity.x != 0)
        {
            animator.SetBool("isRunning", true);
            GetComponentInChildren<Animator>().SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
            GetComponentInChildren<Animator>().SetBool("isRunning", false);
        }

        ///Player Jump Animation

        if (!grounded)
        {
            if (velocity.y > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            else
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
        }
        else
        {
            animator.SetBool("isFalling", false);
        }


        ///Player Wall Slide + Transition
        if (wallSliding)
        {
            animator.SetBool("isWallSliding", true);
            if (!wallDustEffect.isPlaying)
            {
                wallDustEffect.Play();
            }
        }
        else
        {
            animator.SetBool("isWallSliding", false);
            wallDustEffect.Stop();
        }
        
    }

    #endregion

    #region Misc
    void FlipPlayerSprite()
    {
        facingRight = !facingRight;
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
        
    }

    void TestMethod()
    {
        //Used to test if this works while dashing
        Debug.Log("TESTING");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(new Vector3(transform.position.x + .3f * xDirect, transform.position.y, transform.position.z), .1f);
    }
    #endregion

    #region Health Management

    public void Damage(float damage, Transform attackerPos = null)
    {

        healthSystem.Damage((int)damage);

        Vector2 knockbackDir = transform.position - attackerPos.position;
        knockbackDir = knockbackDir.normalized;
        Debug.Log(knockbackDir.y);
        animator.Play("Player_Hit");
        StartCoroutine(KnockbackCo(knockbackDir));

    }

    public void CheckForHealth()
    {

    }

    void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        playerHealthBar.SetHealthFill(healthSystem.GetHealthPercent());
        if (healthSystem.CheckIsDead())
        {
            //Restart the scene if the player dies
        }
    }

    public IEnumerator KnockbackCo(Vector2 knockbackDir)
    {
        float knockTimer = knockbackTime;

        while(knockTimer > 0)
        {
            if (!isKnocked)
            {
                isKnocked = true;
                rb2d.velocity = new Vector2(knockbackDir.x * knockForce, knockbackDir.y + (2 * knockForce));
            }

            rb2d.velocity = Vector2.MoveTowards(rb2d.velocity, Vector2.zero, 2f * Time.deltaTime);

            knockTimer -= 1 * Time.deltaTime;
            yield return null;
        }

        isKnocked = false;

        rb2d.velocity = Vector2.zero;
        
        
    }

    #endregion

    #region Data Management

    public void SaveData(GameData data)
    {
        data.playerPosition = CheckpointManager.instance.currentCheckpointPos;
    }

    public void LoadData(GameData data)
    {
        Debug.Log(data.playerPosition);
        if(CheckpointManager.instance.currentScene == SceneManager.GetActiveScene().name || CheckpointManager.instance.currentScene == "")
        {
            transform.position = data.playerPosition;
        }
        
    }

    #endregion

}
