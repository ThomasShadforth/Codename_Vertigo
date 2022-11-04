using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] CharacterInputController input = null;

    [SerializeField] int currentAttackNum = 1;
    Animator animator;
    PlayerController ownerPlayer;

    bool canAttack = true;
    bool willAttack;

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
        if (Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            return;
        }
        if(input != null)
        {
            willAttack |= input.GetAttackInput();
        }
    }

    public void SetAttack()
    {
        willAttack = true;
    }

    private void FixedUpdate()
    {
        if (willAttack)
        {
            willAttack = false;
            Attack();
        }
    }

    public void Attack()
    {
        if (canAttack)
        {
            //ownerPlayer.isAttacking = true;
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
            /*
            EnemyAIBase enemyAI = enemy.gameObject.GetComponent<EnemyAIBase>();
            enemyAI.GetComponent<IDamageInterface>().Damage(10, ownerPlayer.transform);*/

            EnemyAI enAi = enemy.gameObject.GetComponent<EnemyAI>();

            if (enAi != null)
            {
                enAi.Damage(20, transform);
            }

            ReworkedBaseAI reworkedEnemy = enemy.gameObject.GetComponent<ReworkedBaseAI>();
            reworkedEnemy.Damage(20, transform);
        }

        //Add functionality for attack destructible objects
    }

    public void ResetAnimation()
    {
        //ownerPlayer.isAttacking = false;
        canAttack = true;
        animator.Play("IDLE");
        currentAttackNum = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyAI>())
        {
            EnemyAI AI = other.GetComponent<EnemyAI>();
            AI.Damage(20, transform);
        }

        if (other.GetComponent<ReworkedBaseAI>())
        {
            ReworkedBaseAI AI = other.GetComponent<ReworkedBaseAI>();
            AI.Damage(20, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spearPoint.position, spearPointRadius);
    }
}
