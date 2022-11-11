using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public string nextLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //Set the game to loading the next scene. Prevent the player from moving
            GameManager.instance.LoadLevel(nextLevel);
        }

        if (other.CompareTag("Player"))
        {
            GameManager.instance.LoadLevel(nextLevel);
        }
    }
}
