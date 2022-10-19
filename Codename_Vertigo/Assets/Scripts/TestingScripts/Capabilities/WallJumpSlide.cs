using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpSlide : MonoBehaviour
{
    [SerializeField] GenericInputController input = null;
    [SerializeField, Range(0, 10)] float wallJumpHeight = 3f;
    [SerializeField, Range(0, 1f)] float wallSlideGravity = .5f;

    WallJumpCheck wallJumpCheck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
