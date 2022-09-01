using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10f;
    public Vector3 offset;
    Vector3 smoothedPos;
    Vector3 desiredPos;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    private void FixedUpdate()
    {
        desiredPos = target.position + offset;
        smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = smoothedPos;
    }
}
