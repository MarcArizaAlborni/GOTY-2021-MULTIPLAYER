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
            net.AddEvent(eve);
            currentSec = 0.0f;
        }
    }

}
