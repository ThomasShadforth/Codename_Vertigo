using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : Platform
{
    public Vector2 moveVelocity;
    public Vector2 startPosition;
    public float distanceLimit;

    public float waitTime = 1f;
    bool waiting;

    GameObject target;
    public Vector3 offset;

    

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (waiting || Dialogue_Manager.instance.dialogueIsPlaying || GameManager.instance.isPaused)
        {
            return;
        }
        

        transform.position += (Vector3)moveVelocity * Time.deltaTime;

        float distanceTravelled = Vector2.Distance(startPosition, transform.position);

        if(distanceTravelled >= distanceLimit)
        {
            StartCoroutine(Wait());
        }

        if (target != null)
        {
            //target.GetComponent<Rigidbody2D>().velocity = new Vector2(_rb2d.velocity.x, target.GetComponent<Rigidbody2D>().velocity.y);
        }


    }

    IEnumerator Wait()
    {
        
        waiting = true;
        startPosition = transform.position;
        yield return new WaitForSeconds(waitTime);
        waiting = false;
        moveVelocity *= -1;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            
            if (target == null)
            {
                target = other.gameObject;
                target.transform.parent = transform;
                //offset = target.GetComponent<Rigidbody2D>().velocity - _rb2d.velocity;
            }
        }*/
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            target.transform.parent = null;
            target = null;
        }*/
    }
}
