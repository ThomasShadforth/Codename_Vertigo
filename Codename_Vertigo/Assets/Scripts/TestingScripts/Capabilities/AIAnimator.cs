using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    Animator _animator;
    Rigidbody2D _rb2d;
    Move _move;
    AudioSource _as;

    private void Awake()
    {
        _move = GetComponent<Move>();
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimateAI();
    }

    public void AnimateAI()
    {
        if(_move.xDirection != 0)
        {
            _animator.SetBool("isMoving", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
        }
    }

    public void ResetAnimation()
    {
        _animator.Play("IDLE");
    }

    public void PlaySoundEffect()
    {
        _as.Play();
    }
}
