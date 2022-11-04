using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFSM
{
    idle,
    move
}

public class PlayerStates : MonoBehaviour
{
    public PlayerFSM currentState;

    bool _isMove;

    Move _move;
    Jump _jump;
    CollisionDataCheck _collisionDataCheck;
    Rigidbody2D _rb2d;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _jump = GetComponent<Jump>();
        _rb2d = GetComponent<Rigidbody2D>();
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterState()
    {
        switch (currentState)
        {
            case PlayerFSM.idle:
                _rb2d.isKinematic = true;
                break;
            case PlayerFSM.move:
                _rb2d.isKinematic = false;
                break;
            default:
                break;
        }
        
    }

    
}
