using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
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
        //If the player enters the trigger box
        if (other.gameObject.GetComponent<PlayerController>())
        {
            other.gameObject.GetComponent<PlayerController>().SetDying();
            StartCoroutine(RestartLevelCo());
            //Implement death here - Have the player freeze in place, then play an animation where the player falls or vanishes
            //Restart the level/return to checkpoint
        }
    }

    IEnumerator RestartLevelCo()
    {
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);
        GameManager.instance.RestartScene();
    }
}
