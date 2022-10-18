using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    //Responsible for managing checkpoint throughout levels (if triggered/needed)
    public static CheckpointManager instance;

    public Vector2 currentCheckpointPos;
    public string levelGateName;
    [SerializeField] LevelCheckpoint currentCheckpoint;

    public string currentScene = "";

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name != "Main_Menu")
        {
            if (SceneManager.GetActiveScene().name != currentScene) {

                if (GameManager.instance.loadingGame)
                {
                    GameManager.instance.loadingGame = false;
                    return;
                }

                if (SceneManager.GetActiveScene().name != "Level_Hub")
                {
                    currentCheckpoint = null;
                    GameObject levelStart = GameObject.FindGameObjectWithTag("LevelStart");
                    currentCheckpointPos = levelStart.transform.position;
                    FindObjectOfType<PlayerController>().transform.position = currentCheckpointPos;
                    currentScene = SceneManager.GetActiveScene().name;
                }
                else
                {
                    //Write the code to set the player's position
                    if (levelGateName != "")
                    {
                        LevelGate[] levelGates = GameObject.FindObjectsOfType<LevelGate>();

                        for (int i = 0; i < levelGates.Length; i++)
                        {
                            if (levelGates[i].levelName == levelGateName)
                            {
                                currentCheckpointPos = levelGates[i].gameObject.transform.position;
                                FindObjectOfType<PlayerController>().transform.position = currentCheckpointPos;
                            }
                        }
                    }
                    else
                    {
                        GameObject levelHubStart = GameObject.FindGameObjectWithTag("LevelStart");
                        currentCheckpointPos = levelHubStart.transform.position;
                        FindObjectOfType<PlayerController>().transform.position = currentCheckpointPos;
                    }

                    currentScene = SceneManager.GetActiveScene().name;
                }
            }  
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void SetCurrentCheckpoint(LevelCheckpoint checkpointToSet)
    {
        
        //If a checkpoint has already been found in the current level
        if(currentCheckpoint != null)
        {
            //Set current checkpoint to false (Used to store for the game data for a level)
            currentCheckpoint.isCurrentCheckpoint = false;
            currentCheckpoint.GetComponent<Animator>().SetBool("isActive", false);
        }
        currentCheckpoint = checkpointToSet;
        currentCheckpoint.isCurrentCheckpoint = true;
        currentCheckpoint.GetComponent<Animator>().SetBool("isActive", true);
        currentCheckpointPos = currentCheckpoint.transform.position;
    }

    public void SetPlayerPosAfterDeath()
    {
        //Set the player's transform while loading
        
    }

    
}
