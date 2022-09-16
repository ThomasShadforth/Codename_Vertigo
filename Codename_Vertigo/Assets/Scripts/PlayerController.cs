using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : CustomPhysics, IDataPersistence
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

    public bool isDashing;
    public bool wallJumping;

    public float wallClingRange;
    public int xDirect;
    public LayerMask wallJumpLayer;

    public bool facingRight;
    

    // Start is called before the first frame update
    void Start()
    {
        xDirect = 1;
        facingRight = true;
    }

    // Update is called once per frame
    protected override void ComputeVelocity()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManager.instance.RestartScene();
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

            if (Input.GetMouseButtonDown(0)) {
                TestMethod();
            }
        }

        if(lastKeyPressed != KeyCode.None)
        {
            keyTime -= Time.deltaTime;
            if(keyTime <= 0)
            {
                lastKeyPressed = KeyCode.None;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            TestMethod();
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
        xDirect *= -1;
        velocity.y = jumpHeight * .9f;
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
