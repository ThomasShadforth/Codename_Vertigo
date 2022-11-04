using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    [SerializeField] GenericInputController input = null;
    Jump _jump;
    CollisionDataCheck _collisionDataCheck;
    Move _move;
    WallJumpSlide _wallJump;
    Rigidbody2D _rb2d;

    Animator animator;
    AudioSource _as;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        _jump = GetComponent<Jump>();
        _move = GetComponent<Move>();
        _wallJump = GetComponent<WallJumpSlide>();
        _collisionDataCheck = GetComponent<CollisionDataCheck>();
        _rb2d = GetComponent<Rigidbody2D>();
        _as = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimateObject();
    }

    public void AnimateObject()
    {

        //Debug.Log(_collisionDataCheck._contactNormal.y);

        if(input.GetMoveInput() != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (!_collisionDataCheck._onGround)
        {
            if(_rb2d.velocity.y > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            else
            {
                
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
            }
        }
        else
        {
            
            animator.SetBool("isFalling", false);
        }

        if(_wallJump != null)
        {
            if (_wallJump._onWall)
            {
                animator.SetBool("isWallSliding", true);
            }
            else
            {
                animator.SetBool("isWallSliding", false);
            }
        }

        
    }

    public void PlaySoundEffect()
    {
        _as.Play();
    }
}
