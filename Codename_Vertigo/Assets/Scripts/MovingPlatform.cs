using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
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

        

    }

    IEnumerator Wait()
    {
        waiting = true;
        startPosition = transform.position;
        yield return new WaitForSeconds(waitTime);
        waiting = false;
        moveVelocity *= -1;

    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("AAAAAAAAAAA");
            target = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            target = null;
        }
    }


}
