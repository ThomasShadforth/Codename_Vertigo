using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance { get; private set; }

    public int relicsCollected;

    public string currentScene;

    public bool loadForFirstTime;

    public bool isLoading = false;

    public bool loadingGame = false;

    [SerializeField] Scene currentSceneAsset;

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
        currentSceneAsset = SceneManager.GetActiveScene();
        currentScene = currentSceneAsset.name;
    }

    public void RestartScene()
    {
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckRelicRequirement(int requiredNumber)
    {
        if(relicsCollected >= requiredNumber)
        {
            //Do a thing
            //In this case, unlock the way to the next area/new level
        }
    }

    public void LoadLevel(string levelToLoad)
    {
        StartCoroutine(LoadLevelCo(levelToLoad));
    }

    public void LoadData(GameData data)
    {
        Debug.Log("LOADING DATA ON: " + gameObject.name);
        foreach(KeyValuePair<string, bool> pair in data.relicsDictionary)
        {
            if (pair.Value)
            {
                relicsCollected++;
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.lastSceneSaved = currentScene;
    }

    IEnumerator LoadLevelCo(string levelToLoad)
    {
        isLoading = true;
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(levelToLoad);
        
    }
    
}
