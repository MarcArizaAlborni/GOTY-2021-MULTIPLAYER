using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class LobyUI : MonoBehaviour
{
    public GameObject readyPanel;
    public GameObject notReadyPanel;

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


    



}
