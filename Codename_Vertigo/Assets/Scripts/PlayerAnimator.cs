using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Rigidbody2D _rb2d;
    ReworkedPlayerController _pc;
    Animator _animator;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _pc = GetComponent<ReworkedPlayerController>();
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        AnimateCharacter();
    }

    public void AnimateCharacter()
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }

        if (!_pc.grounded)
        {
            if(_rb2d.velocity.y > 0)
            {
                _animator.SetBool("isJumping", true);
                _animator.SetBool("isFalling", false);
            }

            if(_rb2d.velocity.y < 0)
            {
                _animator.SetBool("isFalling", true);
                _animator.SetBool("isJumping", false);
            }

            if (_pc._onWall)
            {
                _animator.SetBool("isWallSliding", true);
                _animator.SetBool("isJumping", false);
            }
            else
            {
                _animator.SetBool("isWallSliding", false);
            }
        }
        else
        {
            Debug.Log("GROUNDED");

            _animator.SetBool("isFalling", false);
            _animator.SetBool("isJumping", false);
        }
    }
}
