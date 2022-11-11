using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBasic : MonoBehaviour
{

    GameObject _target;
    bool _facingRight = true;
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(_target.transform.position.x > transform.position.x)
        {
            if (!_facingRight)
            {
                //Flip the x scale
                FlipXScale();
            }
        }
        else
        {
            if (_facingRight)
            {
                FlipXScale();
            }
        }
    }

    void FlipXScale()
    {
        Vector3 scalar = transform.localScale;
        scalar.x *= -1;
        transform.localScale = scalar;
        _facingRight = !_facingRight;
    }
}
