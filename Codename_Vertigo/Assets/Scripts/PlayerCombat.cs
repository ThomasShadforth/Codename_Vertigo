using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] int currentAttackNum = 1;
    Animator animator;
    PlayerController ownerPlayer;

    bool canAttack = true;

    [SerializeField] Transform spearPoint;
    [SerializeField] float spearPointRadius;
    public LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ownerPlayer = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        if (canAttack)
        {
            ownerPlayer.isAttacking = true;
            canAttack = false;
            //Add a check to see if the player is grounded (For ground/air attacks)
            animator.Play("Player_Attack_" + currentAttackNum);

            if (animator.HasState(0, Animator.StringToHash("Player_Attack_" + (currentAttackNum + 1))))
            {
                currentAttackNum++;
            }
            else
            {
                
            }
        }
    }

    public void SetAttackWindow(int trueOrFalse)
    {
        if(trueOrFalse == 0)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
    }

    public void DetectAttack()
    {
        Collider2D[] enemiesDetected = Physics2D.OverlapCircleAll(spearPoint.position, spearPointRadius, enemyLayer);

        

        foreach(Collider2D enemy in enemiesDetected)
        {
            EnemyAIBase enemyAI = enemy.gameObject.GetComponent<EnemyAIBase>();
            enemyAI.GetComponent<IDamageInterface>().Damage(10, ownerPlayer.transform);
        }
    }

    public void ResetAnimation()
    {
        ownerPlayer.isAttacking = false;
        canAttack = true;
        animator.Play("IDLE");
        currentAttackNum = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyAIBase>())
        {
            EnemyAIBase enemy = other.GetComponent<EnemyAIBase>();
            enemy.GetComponent<IDamageInterface>().Damage(10, ownerPlayer.transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spearPoint.position, spearPointRadius);
    }
}
