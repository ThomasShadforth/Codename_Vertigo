using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIFSM
{
    Idle,
    Patrol,
    Chase
}

public class EnemyAI : MonoBehaviour, IDamageInterface
{
    [SerializeField] AIInputController input = null;
    [SerializeField] float _minDistFromPoint;
    [SerializeField] float _detectRadius;
    [SerializeField] float _attackRadius;

    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] Transform _target;

    [SerializeField] int _startPatrolPoint;

    [SerializeField] AIFSM _defaultState;

    [SerializeField] int _maxHP;
    [SerializeField] float knockTime;
    [SerializeField] Vector2 knockForce;
    [SerializeField] HealthBar _enemyHealthBar;
    HealthSystem _healthSystem;

    bool _waiting;
    bool _attacking;
    bool _waitingSet;
    bool _isKnocked;
    Move _move;
    Jump _jump;

    Animator _animator;

    AIFSM _currentState;
    int currentPatrolPoint;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _jump = GetComponent<Jump>();
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _healthSystem = new HealthSystem(_maxHP);
        _healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        _currentState = _defaultState;
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isKnocked || _waiting || _attacking || Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            return;
        }

        if(_currentState == AIFSM.Idle)
        {
            _move.SetXDirection(0);
        }

        if(Vector2.Distance(transform.position, _target.position) <= _detectRadius && _currentState != AIFSM.Chase)
        {
            _currentState = AIFSM.Chase;
        }
        else if(Vector2.Distance(transform.position, _target.position) > _detectRadius && _currentState == AIFSM.Chase)
        {
            _currentState = _defaultState;
        }
    }

    private void FixedUpdate()
    {
        if (_waiting || _attacking || _isKnocked || Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            if (_waiting && !_waitingSet)
            {
                _move.SetXDirection(0);
                
                _waitingSet = true;
                StartCoroutine(PatrolWaitCo());
            }
            return;
        }

        if(_currentState == AIFSM.Patrol)
        {
            if (!_waiting)
            {
                float xDirection = transform.position.x < _patrolPoints[currentPatrolPoint].position.x ? 1 : -1;
                _move.SetXDirection(xDirection);
                _waiting = input.Patrol(transform, _patrolPoints[currentPatrolPoint], _minDistFromPoint);
            }
        }

        if(_currentState == AIFSM.Chase)
        {
            if (!_attacking)
            {
                float xDirection = transform.position.x < _target.position.x ? 1 : -1;
                _move.SetXDirection(xDirection);
                _attacking = input.Chase(transform, _target, _attackRadius);

                if (_attacking)
                {
                    _move.SetXDirection(0);
                    _animator.Play("Attack");

                }
            }
        }
    }

    IEnumerator PatrolWaitCo()
    {
        yield return new WaitForSeconds(1f);
        currentPatrolPoint++;

        if(currentPatrolPoint >= _patrolPoints.Length)
        {
            currentPatrolPoint = 0;
        }
        _waiting = false;
        _waitingSet = false;
    }

    public void ResetAttack()
    {
        _attacking = false;
        _animator.Play("IDLE");
    }

    public void Damage(float damage, Transform attackerPos)
    {
        _healthSystem.Damage((int)damage);
        Vector2 knockbackDir = transform.position - attackerPos.position;
        knockbackDir = knockbackDir.normalized;
        knockbackDir.y = .3f;
        _animator.Play("Hit");
        StartCoroutine(KnockbackCo(knockbackDir));
    }

    IEnumerator KnockbackCo(Vector2 knockbackDir)
    {
        float knockCounter = knockTime;
        while(knockCounter > 0)
        {
            if (!_isKnocked)
            {
                _isKnocked = true;
                _attacking = false;
                _move._isKnocked = true;
                _jump._isKnocked = true;
                GetComponent<Rigidbody2D>().velocity = knockbackDir * knockForce;
            }

            GetComponent<Rigidbody2D>().velocity = Vector2.MoveTowards(GetComponent<Rigidbody2D>().velocity, Vector2.zero, Time.deltaTime);
            knockCounter -= 1 * Time.deltaTime;
            yield return null;
        }
        _move._isKnocked = false;
        _jump._isKnocked = false;
        _isKnocked = false;
        
    }

    public void CheckForHealth()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerCharacter>())
        {
            Debug.Log("GOT PLAYER");
            other.GetComponent<PlayerCharacter>().Damage(20, transform);
        }
    }

    void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        _enemyHealthBar.SetHealthFill(_healthSystem.GetHealthPercent());

        if (_healthSystem.CheckIsDead())
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
}
