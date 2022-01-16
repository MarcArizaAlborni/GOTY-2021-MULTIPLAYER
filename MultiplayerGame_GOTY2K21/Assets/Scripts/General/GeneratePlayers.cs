using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlayers : MonoBehaviour
{

    private ClientNetwork2 net;


    public GameObject ourPlayer;
    public GameObject theirPlayer;

    public List<GameObject> playerSpawnPositionsList;


    void Start()
    {
        net = GameObject.FindGameObjectsWithTag("NetObject")[0].GetComponent<ClientNetwork2>();


        for (int i = 0; i < net.spawnPlayers.Count; ++i)
        {

            if (net.clientNet.GetName(net.serverIpep) == net.spawnPlayers[i])
            {

                GameObject go = Instantiate(ourPlayer, playerSpawnPositionsList[i].transform.position, Quaternion.identity);
                go.GetComponentInChildren<TextMesh>().text = net.spawnPlayers[i];
            }
            else
            {
                GameObject go = Instantiate(theirPlayer, playerSpawnPositionsList[i].transform.position, Quaternion.identity);
                go.GetComponentInChildren<TextMesh>().text = net.spawnPlayers[i];
            }
        }
    }

    
}
