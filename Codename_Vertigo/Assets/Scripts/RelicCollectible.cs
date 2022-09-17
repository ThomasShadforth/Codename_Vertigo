using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicCollectible : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    public bool relicCollected;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        //A guid is a 32 character string that has an extremely high chance of being unique
        id = System.Guid.NewGuid().ToString();
    }

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
            relicCollected = true;
            gameObject.SetActive(false);
        }
    }

    public void SaveData(GameData data)
    {
        if (data.relicsDictionary.ContainsKey(id))
        {
            data.relicsDictionary.Remove(id);
        }

        data.relicsDictionary.Add(id, relicCollected);
    }

    public void LoadData(GameData data)
    {
        data.relicsDictionary.TryGetValue(id, out relicCollected);
        if (relicCollected)
        {
            gameObject.SetActive(false);
        }
    }
}
