using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class LobyUI : MonoBehaviour
{
    public GameObject readyPanel;
    public GameObject notReadyPanel;

    private ClientNetwork2 net;
    [SerializeField]private float nameRequestSec = 1.0f;
    private float currentSec = 0.0f;
    private uint lastSeqNum = 0;

    bool isReady = false;

    public List<Text> PlayerStrings;
        
    public void SetReady()
    {
        if (isReady)
        {
            isReady = false;
            notReadyPanel.SetActive(true);
            readyPanel.SetActive(false);
        }
        else
        {
            notReadyPanel.SetActive(false);
            readyPanel.SetActive(true);
            isReady = true;
        }
    }

    private void Start()
    {
        net = GameObject.FindGameObjectsWithTag("NetObject")[0].GetComponent<ClientNetwork2>();
    }

    private void Update()
    {
        currentSec += Time.deltaTime;
        if(currentSec >= nameRequestSec)
        {
            RequestLobbyInfoEvents eve = new RequestLobbyInfoEvents();
            eve.clientReady = isReady;
            eve.forceGameStart = false;
            net.AddEvent(eve);
            currentSec = 0.0f;
        }
    }
    private void OnEnable()
    {
        myNet.lobbyEvent += LobyEventExecution;
    }
    private void OnDisable()
    {
        myNet.lobbyEvent -= LobyEventExecution;
    }
    public void LobyEventExecution(LobbyEvent eve, uint seqNum)
    {
        if (lastSeqNum > seqNum)
            return;
        lastSeqNum = seqNum;
        int i = 0;
        foreach (string name in eve.playerList)
        {
            PlayerStrings[i].text = name;
            ++i;
        }
    }
}
