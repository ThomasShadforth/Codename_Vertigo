using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGate : MonoBehaviour, IDataPersistence
{
    public string levelName;

    public bool isUnlocked;

    bool playerNearPortal;

    public int relicsRequired;

    Animator animator;

    [SerializeField] GameObject effects;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isUnlocked)
            {
                animator.SetBool("isOpen", true);
            }
            else
            {
                GameManager.instance.LoadLevel(levelName);
            }
        }
    }

    public void EnablePortalEffects()
    {
        effects.SetActive(true);
        isUnlocked = true;
    }
    public void SaveData(GameData data)
    {
        //Note: Create a serializable dictionary to store the level gates and their current open status
        if (data.levelGateDictionary.ContainsKey(levelName))
        {
            data.levelGateDictionary.Remove(levelName);
        }

        data.levelGateDictionary.Add(levelName, isUnlocked);
    }

    public void LoadData(GameData data)
    {
        data.levelGateDictionary.TryGetValue(levelName, out isUnlocked);

        if (isUnlocked)
        {
            //Set the animator attached to this object to play the portal opening animation
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>()) {
            playerNearPortal = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            playerNearPortal = false;
        }
    }
}
