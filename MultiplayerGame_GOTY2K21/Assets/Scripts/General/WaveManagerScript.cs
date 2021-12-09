using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerScript : MonoBehaviour
{

    public int currentRoundNum = 1;
   

    public int difficultyModifier=1;

    public GameObject zombiePrefab;
    public List<GameObject> spawnerPointsList;
   [HideInInspector] public List<GameObject> activeZombiesList;
    int zombiesOnWaitCount;


     int zombiesToSpawn = 0;
     int activeZombiesCap = 0;

    private void Start()
    {
        activeZombiesCap = 24 + 6 * (5 - 1); //5 TAKING INTO ACCOUNT WE HAVE 5 PLAYERS ACTIVE
        zombiesOnWaitCount = 0;
    }
    private void Update()
    {
        CheckCurrentRoundState();
    }

    public void StartNewRound()
    {
       int num= CalculateNumberZombies();

        if (num <= activeZombiesCap)
        {
            SpawnNewZombieRound(num);
        }
        else
        {
            SpawnNewZombieRound(activeZombiesCap);
            zombiesOnWaitCount = num - activeZombiesCap;

        }

    }

    public int CalculateNumberZombies() 
    {
        float numF = 0;

        numF = 0.15f * currentRoundNum * activeZombiesCap;

        int num = Mathf.RoundToInt(numF);

        return num;
    }


    public void SpawnNewZombieRound(int numZombies)
    {

        for(int i = 0; i < numZombies; ++i)
        {
            SpawnZombie();
        }

    }

    public void SpawnZombie()
    {
       
        int spawnerNum=Random.Range(1, spawnerPointsList.Count);

        Instantiate(zombiePrefab, spawnerPointsList[spawnerNum].transform);

       
    }

    public void SpawnMidRoundZombie()
    {

        int spawnerNum = Random.Range(1, spawnerPointsList.Count);

        Instantiate(zombiePrefab, spawnerPointsList[spawnerNum].transform);

        --zombiesOnWaitCount;
    }

    public void CheckCurrentRoundState()
    {
        if (activeZombiesList.Count == 0 && zombiesOnWaitCount==0)
        {
            StartNewRound();
            return;
        }

       while (activeZombiesList.Count <= activeZombiesCap && zombiesOnWaitCount!= 0)
       {
            SpawnMidRoundZombie();
       }
    }


    public void ManageDifficulty()
    {

    }

}
