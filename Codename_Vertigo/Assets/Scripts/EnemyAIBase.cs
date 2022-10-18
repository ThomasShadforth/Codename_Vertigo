using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyFSM
{
    idle,
    patrol,
    pursuit,
    attack
}
public class EnemyAIBase : CustomPhysics, IDamageInterface, IKnockbackInterface
{
    public float enemyMaxHP;
    

    [Header("Movement Values")]
    public float patrolSpeed;
    public float pursuitSpeed;
    public float jumpHeight;

    [Header("Player Target")]
    public Transform targetPlayer;
    
    [Header("Start Position")]
    public Vector3 startingPoint;
    bool atStart = false;

    [Header("Patrol Properties")]
    public Transform[] patrolPoints;
    public int startingPatrolIndex;
    public float minDistanceFromPoint;
    int currentPatrolIndex;
    bool waiting;

    [Header("Additional Properties - Detection Radius, Jump Properties, etc.")]
    public float detectRadius;
    public float minDistToAttack;
    public float minDistToJump;
    public bool canJump;
    public float knockForce;
    public float knockbackTimer;

    public EnemyFSM currentState;
    EnemyFSM defaultState;

    [SerializeField] HealthBar enemyHealthBar;
    HealthSystem healthSystem;
    Animator animator;
    int xDirect = 1;
    protected bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        currentPatrolIndex = startingPatrolIndex;
        defaultState = currentState;
        startingPoint = transform.position;
        targetPlayer = FindObjectOfType<PlayerController>().gameObject.transform;
        healthSystem = new HealthSystem((int)enemyMaxHP);
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        animator = GetComponent<Animator>();
        
    }

    

    // Update is called once per frame
    protected override void ComputeVelocity()
    {
        if (Dialogue_Manager.instance.dialogueIsPlaying)
        {
            if(animator.speed > 0)
            {
                animator.speed = 0;
            }
            return;
        }
        else
        {
            if(animator.speed < 1)
            {
                animator.speed = 1f;
            }
        }

        if (isAttacking || isKnocked || GameManager.instance.isPaused)
        {
            return;
        }

        Vector2 move = Vector2.zero;

        
        if (currentState == EnemyFSM.patrol)
        {
            EnemyPatrol(move);
        }
        else if (currentState == EnemyFSM.pursuit)
        {
            EnemyPursuit(move);
        }
        

        //Check whether or not the player is in range of the enemy. Will only trigger once when the player enters it's range
        if(currentState != EnemyFSM.pursuit && Vector2.Distance(rb2d.position, targetPlayer.position) <= detectRadius)
        {
            //Set state to the pursuit state
            currentState = EnemyFSM.pursuit;
        }
        //Check if the player leaves the enemy's range. Will only trigger once when the player leaves.
        else if(currentState == EnemyFSM.pursuit && Vector2.Distance(rb2d.position, targetPlayer.position) > detectRadius)
        {
            //Set state to default
            currentState = defaultState;
        }

        AnimateEnemy();
    }

    #region Patrol Methods/Coroutines
    protected virtual void EnemyPatrol(Vector2 move)
    {
        
        FlipXScale(patrolPoints[currentPatrolIndex]);

        if (!waiting)
        {
            //rb2d.position = Vector2.MoveTowards(rb2d.position, patrolPoints[currentPatrolIndex].position, patrolSpeed * Time.deltaTime);
            float distance = Mathf.Abs(rb2d.position.x) - Mathf.Abs(patrolPoints[currentPatrolIndex].position.x);

            if (patrolPoints[currentPatrolIndex].position.x > rb2d.position.x)
            {
                move.x = 1;
            }
            else
            {
                move.x = -1;
            }

            targetVelocity = move * patrolSpeed;

            

            distance = Mathf.Abs(distance);

            if (distance <= minDistanceFromPoint)
            {
                currentPatrolIndex++;

                if (currentPatrolIndex >= patrolPoints.Length)
                {
                    currentPatrolIndex = 0;
                }

                StartCoroutine(EnemyPatrolWait(2f));
            }

        }
        

        
    }

    protected virtual IEnumerator EnemyPatrolWait(float waitDur)
    {
        waiting = true;
        yield return new WaitForSeconds(waitDur);
        waiting = false;
    }
    #endregion

    #region Pursuit methods/Coroutines
    protected virtual void EnemyPursuit(Vector2 move)
    {
        
        FlipXScale(targetPlayer);

        float directionOfPlayerX = Mathf.Abs(rb2d.position.x) - Mathf.Abs(targetPlayer.position.x);

        float distanceAbs;
        if (targetPlayer.position.x > rb2d.position.x)
        {
            move.x = 1;
        }
        else
        {
            move.x = -1;
        }

        targetVelocity = move * pursuitSpeed;

        

        distanceAbs = Mathf.Abs(directionOfPlayerX);
        

        if (canJump)
        {
            if (targetPlayer.position.y > rb2d.position.y && distanceAbs <= minDistToJump)
            {
                if (grounded)
                {
                    velocity.y = jumpHeight;
                }
            }
        }

        if (distanceAbs <= minDistToAttack)
        {
            if (!isAttacking)
            {
                animator.Play("Attack");
                isAttacking = true;
            }
        }
        

    }
    #endregion

    #region Health Management

    public virtual void Damage(float damageDealt, Transform attackPos = null)
    {
        healthSystem.Damage((int)damageDealt);

        Vector2 knockbackDir = transform.position - attackPos.position;
        animator.Play("Hit");
        knockbackDir = knockbackDir.normalized;
        Debug.Log(knockbackDir);
        StartCoroutine(KnockbackCo(knockbackDir));

    }

    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        enemyHealthBar.SetHealthFill(healthSystem.GetHealthPercent());
        if (healthSystem.CheckIsDead())
        {
            FindObjectOfType<HitStopController>().StopTime(.1f);
            Destroy(gameObject);
        }
    }

    public virtual void CheckForHealth()
    {
        /*if(enemyHP <= 0)
        {
            FindObjectOfType<HitStopController>().StopTime(.1f);
            Destroy(gameObject);
        }*/
    }

    public IEnumerator KnockbackCo(Vector2 knockbackDir)
    {
        float knockTimer = knockbackTimer;

        while(knockTimer> 0)
        {
            if (!isKnocked)
            {
                isKnocked = true;
                rb2d.velocity = new Vector2(knockbackDir.x, knockbackDir.y + 2);
            }

            rb2d.velocity = Vector2.MoveTowards(rb2d.velocity, Vector2.zero, 2f * Time.deltaTime);
            knockTimer -= 1 * Time.deltaTime;
            yield return null;
        }

        isKnocked = false;
        rb2d.velocity = Vector2.zero;
    }

    #endregion

    #region Animate Character
    
    void AnimateEnemy()
    {
        

        if(targetVelocity.x != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void FlipXScale(Transform targetObject)
    {
        

        if(targetObject.position.x < rb2d.position.x)
        {
            if(xDirect == 1)
            {
                xDirect = -1;
                Vector3 scalar = transform.localScale;
                scalar.x = scalar.x * -1;
                transform.localScale = scalar;
            }
        }
        else if(targetObject.position.x > rb2d.position.x)
        {
            if(xDirect == -1)
            {
                xDirect = 1;
                Vector3 scalar = transform.localScale;
                scalar.x = scalar.x * -1;
                transform.localScale = scalar;
            }
        }
    }

    void ResetAnimation()
    {
        currentState = defaultState;
        animator.Play("IDLE");
        isAttacking = false;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.Damage(20, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, minDistToJump);
    }
}
