using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicDoor : MonoBehaviour, IDataPersistence
{
    public string doorName;

    public bool isOpen;

    public bool isBossRelicDoor;

    public string bossRelicNeeded;

    public int relicsNeeded;

    [SerializeField] ParticleSystem RelicDoorDust;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            isOpen = true;
            animator.SetBool("isOpen", isOpen);
        }
    }

    public void CheckRelicRequirements()
    {
        //Check the player's held relics. If they have enough, open the door
        if(GameManager.instance.relicsCollected >= relicsNeeded)
        {
            isOpen = true;
            animator.SetBool("isOpen", true);
        }

    }

    public void SaveData(GameData data)
    {
        if (data.hubBarrierDictionary.ContainsKey(doorName))
        {
            data.hubBarrierDictionary.Remove(doorName);
        }

        data.hubBarrierDictionary.Add(doorName, isOpen);
    }

    public void LoadData(GameData data)
    {
        data.hubBarrierDictionary.TryGetValue(doorName, out isOpen);

        if (isOpen)
        {
            
            animator.SetBool("isOpen", true);
            //Set the animator to play the door opening animation
        }
    }

    public void DeactivateCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    public void PlayDustEffect()
    {
        RelicDoorDust.Play();
    }
}
