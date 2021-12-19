using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobyUI : MonoBehaviour
{
    public GameObject readyPanel;
    public GameObject notReadyPanel;

    bool isReady = false;

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
