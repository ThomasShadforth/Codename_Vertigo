using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCheckpoint : MonoBehaviour, IDataPersistence
{
    public bool isCurrentCheckpoint;
    Vector2 position;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            if (!isCurrentCheckpoint)
            {
                CheckpointManager.instance.SetCurrentCheckpoint(this);
                DataPersistenceManager.instance.SaveGame();
            }
        }

        if (other.CompareTag("Player"))
        {
            if (!isCurrentCheckpoint)
            {
                CheckpointManager.instance.SetCurrentCheckpoint(this);
                DataPersistenceManager.instance.SaveGame();
            }
        }
    }

    public void SaveData(GameData data)
    {

    }
    public void LoadData(GameData data)
    {

    }
}
