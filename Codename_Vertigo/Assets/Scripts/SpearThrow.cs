using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearThrow : MonoBehaviour
{
    public GameObject spearToThrow;
    public float throwingStrength;
    public float throwPower;

    [SerializeField]
    GameObject throwDot;
    GameObject[] throwingArcDots;
    [SerializeField]
    Transform throwPosition;
    [SerializeField]
    Transform throwingPivot;
    Vector2 throwDirection;

    [SerializeField]
    Spear playerSpear;
    [SerializeField]
    SpearNorm testSpear;

    public int numberOfDots;
    public float distanceBetweenDots;



    //Prevents the player from repeatedly throwing the spear if they've thrown it out already.
    bool spearThrown;

    PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();

        throwingArcDots = new GameObject[numberOfDots];

        for(int i = 0; i < numberOfDots; i++)
        {
            throwingArcDots[i] = Instantiate(throwDot, throwPosition.position, Quaternion.identity);
            throwingArcDots[i].transform.parent = transform;
            throwingArcDots[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Get the start position of the throw arc.
        Vector2 startThrowPos = throwingPivot.position;
        //Get the current mouse position in the world
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseDistFromPlayer = Mathf.Abs(transform.position.x - mousePosition.x);

        //Get the direction of the throw, and multiply it by the current direction (Ensure the aim remains consistent when facing left or right)
        throwDirection = mousePosition - startThrowPos;
        throwDirection = throwDirection * controller.xDirect;
        //Set the right direction of the throw pivot to the direction
        throwingPivot.transform.right = throwDirection;

        if (!spearThrown)
        {
            //When holding down right mouse
            if (Input.GetMouseButton(1))
            {
                //Get the throw power by checking the distance between the player and the mouse position
                throwPower = Vector2.Distance(transform.position, mousePosition);
                //Restrict it to a range between 0 and 5
                throwPower = Mathf.Clamp(throwPower, 0, 5);

                //Loop through the throwing arc dot array, if a dot isn't active, set it active
                for (int i = 0; i < numberOfDots; i++)
                {
                    if (!throwingArcDots[i].activeInHierarchy)
                    {
                        throwingArcDots[i].SetActive(true);
                    }

                    //Set the position of the dot based on the distance between each on (The distance is multiplied by the index to space it out further
                    throwingArcDots[i].transform.position = PointPosition(i * distanceBetweenDots);

                    /*
                    if(Physics2D.OverlapCircle(throwingArcDots[i].transform.position, .5f))
                    {
                        for(int j = i; j < numberOfDots; j++)
                        {
                            throwingArcDots[j].gameObject.SetActive(false);
                        }
                        Debug.Log("DETECTING COLLIDER AT: " + throwingArcDots[i].name + i);
                    }*/

                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                ThrowSpear();
                //For now, make the dots disappear. This will throw the spear eventually.
                for (int i = 0; i < numberOfDots; i++)
                {
                    throwingArcDots[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                Invoke("TriggerRetrieveSpear", .2f);
            }
        }

        
    }

    void ThrowSpear()
    {
        /*
        Spear newSpear = Instantiate(playerSpear, throwPosition.position, throwPosition.rotation);
        newSpear.directionOfSpear = (throwPosition.transform.right * (throwingStrength * throwPower) * controller.xDirect);*/

        SpearNorm newSpear = Instantiate(testSpear, throwPosition.position, throwingPivot.rotation);
        newSpear.GetComponent<Rigidbody2D>().velocity = throwPosition.transform.right * (throwingStrength * throwPower) * controller.xDirect;

        spearThrown = true;

        
    }

    void TriggerRetrieveSpear()
    {
        
        SpearNorm spear = FindObjectOfType<SpearNorm>();

        if (!spear.isPickup)
        {
            spear.ReturnSpear();
            spearThrown = false;
        }
    }

    Vector2 PointPosition(float t)
    {
        //Initialise a new vector 2. Set it to be equal to position + direction.normalised * (throwingStrenght * throwPower) * playerDirection, which is multiplied by t. Then add .5, multiplied by gravity (for the arc) and multiply that by t squared.
        //Gravity factors in where an object would fall along an arc.
        //Base throwing strength is multiplied by power, which affects the overall throwing arc
        Vector2 position = (Vector2)throwPosition.position + ((throwDirection.normalized * (throwingStrength * throwPower) * controller.xDirect) * t) + .5f * Physics2D.gravity * (t * t);
        //Return the position and set the dot to it
        return position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<SpearNorm>() || other.GetComponent<Spear>())
        {
            Destroy(other.gameObject);
            spearThrown = false;
        }
        
    }
}
