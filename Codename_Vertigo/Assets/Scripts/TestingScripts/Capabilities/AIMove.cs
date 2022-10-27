using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStates
{
    Idle,
    Patrol,
    Chase
}


public class AIMove : MonoBehaviour
{
    [SerializeField] GenericInputController input = null;

    [SerializeField] float _moveModifier = 1f;
    [SerializeField] [Range(0, 10f)] float _patrolSpeed;
    [SerializeField] [Range(0, 10f)] float _pursuitSpeed;
    [SerializeField] [Range(0, 10f)] float _playerDetectRadius;
    [SerializeField] Transform _target;
    [SerializeField] Transform[] _patrolPoints;
    [SerializeField] int _startingPatrolIndex;
    [SerializeField] [Range(0, 10f)] float minDistFromPoint;
    int _currentPatrolIndex;
    [SerializeField] AIStates _defaultState;

    Vector2 _direction;

    Jump _jump;
    CollisionDataCheck _collisionDataCheck;

    AIStates _currentState;
    bool _waiting;

    

    private void Awake()
    {
        _jump = GetComponent<Jump>();
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentState = _defaultState;
        _currentPatrolIndex = _startingPatrolIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (_waiting)
        {
            return;
        }

        if(_currentState == AIStates.Patrol)
        {
            //Get the input
            _direction.x = SetMoveDirection(_patrolPoints[_currentPatrolIndex]);
            
        }

        else if(_currentState == AIStates.Chase)
        {
            //Get the input, multiply it based
            _direction.x = SetMoveDirection(_target);
        }
    }

    private void FixedUpdate()
    {
        if (_waiting) {
            return;
        }

        if(_currentState == AIStates.Patrol)
        {
            Patrol();
        }

        if(_currentState == AIStates.Chase)
        {
            
        }

    }

    float SetMoveDirection(Transform targetToTrack)
    {
        return input.GetMoveInput() * targetToTrack.transform.position.x < transform.position.x ? -1 : 1;
    }

    void Patrol()
    {
        float distance = Mathf.Abs(transform.position.x) - Mathf.Abs(_patrolPoints[_currentPatrolIndex].position.x);
        distance = Mathf.Abs(distance);

        if(distance >= minDistFromPoint)
        {
            _currentPatrolIndex++;
            if(_currentPatrolIndex > _patrolPoints.Length)
            {
                _currentPatrolIndex = 0;
            }

            StartCoroutine(EnemyWaitCo());
        }
    }

    IEnumerator EnemyWaitCo()
    {
        _waiting = true;
        yield return new WaitForSeconds(1f);
        _waiting = false;
    }
}
