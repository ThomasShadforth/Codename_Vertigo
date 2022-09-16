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
public class EnemyAIBase : CustomPhysics, IDamageInterface
{
    public float enemyMaxHP;
    float enemyHP;

    public float patrolSpeed;
    public float pursuitSpeed;
    public float jumpHeight;

    public Transform targetPlayer;
    
    public Vector3 startingPoint;
    bool atStart = false;


    public Transform[] patrolPoints;
    public int startingPatrolIndex;
    public float minDistanceFromPoint;
    int currentPatrolIndex;
    bool waiting;

    public float detectRadius;
    public float minDistToJump;
    public bool canJump;
    

    public EnemyFSM currentState;
    EnemyFSM defaultState;
    // Start is called before the first frame update
    void Start()
    {
        currentPatrolIndex = startingPatrolIndex;
        defaultState = currentState;
        startingPoint = transform.position;
        targetPlayer = FindObjectOfType<PlayerController>().gameObject.transform;
        enemyHP = enemyMaxHP;

        
    }

    // Update is called once per frame
    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        if(currentState == EnemyFSM.patrol)
        {
            EnemyPatrol(move);
        } else if(currentState == EnemyFSM.pursuit)
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
    }

    #region Patrol Methods/Coroutines
    protected virtual void EnemyPatrol(Vector2 move)
    {
        

        if (!waiting) {
            //rb2d.position = Vector2.MoveTowards(rb2d.position, patrolPoints[currentPatrolIndex].position, patrolSpeed * Time.deltaTime);
            float distance = Mathf.Abs(rb2d.position.x) - Mathf.Abs(patrolPoints[currentPatrolIndex].position.x);

            if(patrolPoints[currentPatrolIndex].position.x > rb2d.position.x)
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

                if(currentPatrolIndex >= patrolPoints.Length)
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

    }
    #endregion

    #region Health Management

    public virtual void Damage(float damageDealt)
    {
        enemyHP -= damageDealt;

        CheckForHealth();
    }

    public virtual void CheckForHealth()
    {
        if(enemyHP <= 0)
        {
            FindObjectOfType<HitStopController>().StopTime(.1f);
            Destroy(gameObject);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, minDistToJump);
    }
}
