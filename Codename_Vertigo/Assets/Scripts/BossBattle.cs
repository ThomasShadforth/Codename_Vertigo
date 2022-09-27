using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{
    public enum Boss_Phases
    {
        WaitingToStart,
        Phase_1,
        Phase_2,
        Phase_3,
    }

    //To do: Add reference to battle trigger

    //If necessary, add enemies to spawn
    [SerializeField] GameObject enemyToSpawn;

    List<Vector3> spawnPositions;

    public Boss_Phases currentPhase;

    List<GameObject> enemiesList;

    private void Awake()
    {
        spawnPositions = new List<Vector3>();

        foreach(Transform spawnPos in transform.Find("spawnPositions"))
        {
            spawnPositions.Add(spawnPos.position);
        }

        currentPhase = Boss_Phases.WaitingToStart;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBattle()
    {
        StartNextPhase();
        InvokeRepeating("SpawnNewEnemy", 0f, 1.5f);
    }

    void Boss_OnDamaged()
    {
        switch (currentPhase)
        {
            case Boss_Phases.Phase_1:
                //Check if health reaches threshold, then switch phase if it has
                break;
            case Boss_Phases.Phase_2:
                break;
            
        }
    }

    void OnBossBattleEnd()
    {
        DestroyAllEnemies();
        CancelInvoke("SpawnNewEnemy");
    }

    void StartNextPhase()
    {
        switch (currentPhase)
        {
            case Boss_Phases.WaitingToStart:
                currentPhase = Boss_Phases.Phase_1;
                break;
            case Boss_Phases.Phase_1:
                currentPhase = Boss_Phases.Phase_2;
                break;
            case Boss_Phases.Phase_2:
                currentPhase = Boss_Phases.Phase_3;
                break;
            
        }
    }

    void SpawnNewEnemy()
    {
        int aliveCount = 0;
        foreach(GameObject spawnedEnemy in enemiesList)
        {
            //If alive, add to the list

            if(aliveCount >= 6)
            {
                //don't spawn any more enemies
                return;
            }
        }

        Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];

        ///To do:
        ///enemySpawn = defaultEnemy
        ///if(random < 65) enemySpawn = bruiser
        ///if(random < 15) enemySpawn = ranger

        GameObject enemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        //In a script, call a spawn method

        enemiesList.Add(enemy);
    }

    void DestroyAllEnemies()
    {
        foreach(GameObject enemy in enemiesList)
        {
            //kill the enemy if they are alive
        }
    }
}
