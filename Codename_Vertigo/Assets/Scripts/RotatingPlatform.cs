using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : Platform
{
    [SerializeField] Transform _rotationPivot;
    [SerializeField] float _rotationSpeed;
    [SerializeField] Vector3 _rotationAxis;

    [SerializeField] GameObject _target;

    [SerializeField] Vector3 _offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            return;
        }*/

        transform.RotateAround(_rotationPivot.position, Vector3.forward, _rotationSpeed * Time.deltaTime);
        transform.eulerAngles = Vector3.zero;
    }

    private void FixedUpdate()
    {
        
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            
            _target = other.gameObject;
            //_target.transform.parent = transform;

            _offset = _target.transform.position - transform.position;
        }*/
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            _target.transform.parent = null;
            _target = null;
        } */
    }

    private void LateUpdate()
    {
        
    }

}
