using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManagerScript : MonoBehaviour
{
    private ServerNetwork2 net;
    public int currentRoundNum = 0;
   

    public int difficultyModifier=1;

    public GameObject zombiePrefab;
    public List<GameObject> spawnerPointsList;
   [HideInInspector] public List<GameObject> activeZombiesList;
    int zombiesOnWaitCount;
    public int numPlayers = 5;

   
     int activeZombiesCap = 0;


    public List<GameObject> playerSpawnPositionsList;


    float preStartTimer = 5.0f;
    float currStartTimer = 0.0f;

    public GameObject theirPlayer;



    private void Start()
    {
        activeZombiesCap = 24 + 6 * (numPlayers - 1); //5 TAKING INTO ACCOUNT WE HAVE 5 PLAYERS ACTIVE
        zombiesOnWaitCount = 0;
        currentRoundNum=0;

        net = GameObject.FindGameObjectsWithTag("NetObject")[0].GetComponent<ServerNetwork2>();
        List<string> spawnPlayers = net.serverNetwork.GetAllNames();
        net.playerTransforms = new OnlineMovement[spawnPlayers.Count];

        for (int i = 0; i < spawnPlayers.Count; ++i)
        {
            GameObject go = Instantiate(theirPlayer, playerSpawnPositionsList[i].transform.position, Quaternion.identity);
            go.GetComponentInChildren<TextMesh>().text = spawnPlayers[i];
            net.playerTransforms[i] = go.GetComponentInChildren<OnlineMovement>();
        }
    }



    private void Update()
    {

        if(currStartTimer>= preStartTimer)
        {
            CheckCurrentRoundState();
            currStartTimer = 0.0f;
        }
        else
        {
            currStartTimer += Time.deltaTime;

        }

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
        Debug.Log(zombiesOnWaitCount + "Zombies on wait this round");
    }

    public int CalculateNumberZombies() 
    {
        float numF = 0;

        numF = 0.18f * currentRoundNum * activeZombiesCap;

        
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
       Vector2 vecCircl= Random.insideUnitCircle * 1;
       int spawnerNum=Random.Range(1, spawnerPointsList.Count);

        Vector3 finalSpawnPos;

        finalSpawnPos.x= spawnerPointsList[spawnerNum].transform.localPosition.x + vecCircl.x;
        finalSpawnPos.z = spawnerPointsList[spawnerNum].transform.localPosition.z + vecCircl.y;
        finalSpawnPos.y = 0.1f;


        activeZombiesList.Add( Instantiate(zombiePrefab, finalSpawnPos, Quaternion.identity));

        activeZombiesList[activeZombiesList.Count - 1].GetComponent<MovementEnemy>().target=GameObject.Find("model").transform;
    }

    public void SpawnMidRoundZombie()
    {

        Vector2 vecCircl = Random.insideUnitCircle * 1;
        int spawnerNum = Random.Range(1, spawnerPointsList.Count);

        Vector3 finalSpawnPos;

        finalSpawnPos.x = spawnerPointsList[spawnerNum].transform.localPosition.x + vecCircl.x;
        finalSpawnPos.z = spawnerPointsList[spawnerNum].transform.localPosition.z + vecCircl.y;
        finalSpawnPos.y = 0.1f;


        activeZombiesList.Add(Instantiate(zombiePrefab, finalSpawnPos, Quaternion.identity));

        activeZombiesList[activeZombiesList.Count - 1].GetComponent<MovementEnemy>().target = GameObject.Find("model").transform;

        --zombiesOnWaitCount;
        Debug.Log(zombiesOnWaitCount + "remianing this round on wait");
    }
    public void CheckZombieStatus()
    {
        for(int i= activeZombiesList.Count; i>0; --i)
        {
            if (activeZombiesList[i-1] == null)
            {
                //Dead animation can't be played just before erase the GameObject
                //activeZombiesList[i - 1].GetComponent<MovementEnemy>().SetDead();
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
