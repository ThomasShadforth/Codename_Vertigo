using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewParallaxController : MonoBehaviour
{
    public float multiplier;
    public float vertMult;

    public bool horOnly;

    public bool calculateInfiniteHorPos;
    public bool calculateInfiniteVertPos;

    public bool isInfinite;

    public Camera cam;

    public float smoothingFactor;

    private Vector3 startPosition;
    private Vector3 startCamPos;
    private float length;
    Vector3 desiredPos;
    Vector3 smoothedPos;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        startPosition = transform.position;
        startCamPos = cam.transform.position;

        if (isInfinite)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        CalculateStartPos();

    }

    void CalculateStartPos()
    {
        float distX = (cam.transform.position.x - transform.position.x) * multiplier;
        float distY = (cam.transform.position.y - transform.position.y) * multiplier;

        Vector3 tmp = new Vector3(startPosition.x, startPosition.y, transform.position.z);


        if (calculateInfiniteHorPos)
        {
            tmp.x = transform.position.x + distX;
        }

        if (calculateInfiniteVertPos)
        {
            tmp.y = transform.position.y + distY;
        }

        startPosition = tmp;

    }

    private void FixedUpdate()
    {
        Vector3 pos = startPosition;

        if (horOnly)
        {
            pos.x += multiplier * (cam.transform.position.x - startCamPos.x);
        }
        else
        {
            pos.x += (multiplier != 0 ? multiplier : 0) * (cam.transform.position.x - startCamPos.x);
            pos.y += (multiplier != 0 ? multiplier : vertMult) * (cam.transform.position.y - startCamPos.y);
            pos.z += multiplier * (cam.transform.position.z - startCamPos.z);
            //pos += multiplier * (cam.transform.position - startCamPos);
        }

        transform.position = pos;

        if (isInfinite)
        {
            float tmp = cam.transform.position.x * (1 - multiplier);

            if (tmp > startPosition.x + length)
            {
                startPosition.x += length;
            }
            else if (tmp < startPosition.x - length)
            {
                startPosition.x -= length;
            }
        }
    }

    private void LateUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
