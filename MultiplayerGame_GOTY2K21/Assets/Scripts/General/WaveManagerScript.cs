using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerScript : MonoBehaviour
{

    public int currentRoundNum = 0;
   

    public int difficultyModifier=1;

    public GameObject zombiePrefab;
    public List<GameObject> spawnerPointsList;
   [HideInInspector] public List<GameObject> activeZombiesList;
    int zombiesOnWaitCount;


   
     int activeZombiesCap = 0;

    private void Start()
    {
        activeZombiesCap = 24 + 6 * (5 - 1); //5 TAKING INTO ACCOUNT WE HAVE 5 PLAYERS ACTIVE
        zombiesOnWaitCount = 0;
        currentRoundNum=0;
    }
    private void Update()
    {
        CheckCurrentRoundState();
    }

    public void StartNewRound()
    {
        ++currentRoundNum;
        Debug.Log("Round " + currentRoundNum + " " + "Started");
      
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
        Debug.Log(num + "Zombies spawned this round");
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

        activeZombiesList.Add( Instantiate(zombiePrefab, spawnerPointsList[spawnerNum].transform.localPosition,Quaternion.identity));

       
    }

    public void SpawnMidRoundZombie()
    {

        int spawnerNum = Random.Range(1, spawnerPointsList.Count);

        Instantiate(zombiePrefab, spawnerPointsList[spawnerNum].transform);

        --zombiesOnWaitCount;
    }
    public void CheckZombieStatus()
    {
        for(int i= activeZombiesList.Count; i>0; --i)
        {
            if (activeZombiesList[i-1] == null)
            {
                activeZombiesList.RemoveAt(i-1);
            }
        }
    }
    public void CheckCurrentRoundState()
    {

        CheckZombieStatus();


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
