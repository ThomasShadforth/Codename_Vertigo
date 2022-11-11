using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to keep track of the current state of the AI
public enum EnemyAIFSM
{
    idle,
    patrol,
    chase
}

public class ReworkedBaseAI : MonoBehaviour, IDamageInterface
{

    //Movement Values:
    [Header("Movement Values:")]
    public float _patrolMoveSpeed;
    public float _chaseMoveSpeed;
    public float acceleration;
    protected float _maxSpeedChange;
    protected Vector2 _desiredVelocity;
    protected bool _isAttacking;
    protected float direction;

    //Jump Values (If applicable, based on the enemy type):
    [Header("Jump Values:")]
    public float jumpGravity;
    public float fallGravity;
    public float jumpHeight;
    public float coyoteTime;
    public float jumpBuffer;
    public int maxJumpCount;
    protected float _defaultGravity = 1f;
    protected bool _isJumping;
    protected bool _desiredJump = false;
    protected bool _prevGrounded = false;
    protected float _coyoteCounter;
    protected float _jumpBufferCounter;
    protected int _jumpPhase;

    [Header("Patrol Properties:")]
    [SerializeField] protected Transform[] _patrolPoints;
    protected int _currentPatrolIndex;
    protected bool _waiting;

    //Defining distance values:
    [Header("Distance Values:")]
    public float minDistToPatrol;
    public float minDistToJump;
    public float chaseDist;
    public float attackDist;

    //Health Values
    [Header("Health Values:")]
    protected HealthSystem _healthSystem;
    [SerializeField] protected HealthBar _healthBar;
    public int enemyMaxHP;

    [Header("Knockback Values:")]
    public Vector2 knockForce;
    public float knockTimer;

    //Other toggles:
    [Header("Toggles")]
    public EnemyAIFSM currentState;
    public bool canJump;
    protected EnemyAIFSM _defaultState;
    
    public bool isKnocked;

    //Other components
    protected Rigidbody2D _rb2d;
    protected Animator _animator;
    protected Transform _target;
    protected Vector2 _velocity;
    protected Vector2 _gravForce;

    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _gravForce = Physics2D.gravity * _rb2d.mass;
        _animator = GetComponent<Animator>();
        _healthSystem = new HealthSystem(enemyMaxHP);
        _healthSystem.OnHealthChanged += HealthSystem_OnHealhChanged;

        if(_healthSystem != null)
        {
            Debug.Log("HEALTH SYSTEM FOUND");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _defaultState = currentState;
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
    }

    //Monitors the conditions for changing AI states (From idle/patrol to chase, and vice versa)
    protected virtual void CheckState()
    {

    }

    //Executed whilst in the patrol state
    protected virtual void Patrol()
    {

    }

    //Executed while in the chase state

    protected virtual void Chase()
    {

    }

    protected virtual void HealthSystem_OnHealhChanged(object sender, System.EventArgs e)
    {
        //Debug.Log(_healthSystem.GetHealth());
        _healthBar.SetHealthFill(_healthSystem.GetHealthPercent());

        if (_healthSystem.CheckIsDead())
        {
            Destroy(gameObject);
        }
    }

    public virtual void Damage(float damage, Transform attackerPos)
    {
        _healthSystem.Damage((int)damage);
        Vector2 knockDir = transform.position - attackerPos.position;
        knockDir = knockDir.normalized;
        knockDir.y = .3f;
        _animator.Play("Hit");
        StartCoroutine(KnockbackCo(knockDir));
    }

    protected IEnumerator KnockbackCo(Vector2 knockDir)
    {
        float knockCounter = knockTimer;

        while(knockCounter > 0)
        {
            if (!isKnocked)
            {
                isKnocked = true;
                _isAttacking = false;
                _isJumping = false;
                _rb2d.velocity = new Vector2(knockDir.x * knockForce.x, knockDir.y * knockForce.y);
            }

            knockCounter -= Time.deltaTime;

            yield return null;
        }

        isKnocked = false;
    }

    public virtual void CheckForHealth()
    {

    }


}
