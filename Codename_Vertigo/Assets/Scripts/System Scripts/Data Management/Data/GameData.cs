using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Stores a binary value of when this data was last saved/updated
    public long lastSaved;

    //How many lives the player currently has
    public int CurrentLives;

    //How many relics the player has collected
    public int relicsCollected;

    //Player's current position
    public Vector3 playerPosition;
    //Stores a dictionary of relics, and whether or not the player has collected them
    public SerializableDictionary<string, bool> relicsDictionary;
    //Stores the name of the last scene the player saved in (Will be used for checking whether or not loading into different scenes via continue works)
    public string lastSceneSaved;


    //Values defined in the constructor will be their default values
    public GameData()
    {
        this.CurrentLives = 3;
        this.relicsCollected = 0;
        this.playerPosition = new Vector3(-.29f, 0.38f, 0);
        this.relicsDictionary = new SerializableDictionary<string, bool>();
        this.lastSceneSaved = "";
    }

    public int GetPercentageComplete()
    {
        int totalCollected = 0;
        foreach(bool collected in relicsDictionary.Values)
        {
            if (collected)
            {
                totalCollected++;
            }
        }

        int percentageCompleted = 0;

        if(relicsDictionary.Count != 0)
        {
            percentageCompleted = (totalCollected * 100 / relicsDictionary.Count);
        }
        return percentageCompleted;
    }
}
