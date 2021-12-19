using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockDoor : MonoBehaviour
{

    public MoneyManager moneyScript;
    public int moneyRequired = 0;

    public Text interactionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           // Debug.Log("Press E to Open Door");
            interactionText.text = "Press E to open Door";
        }

       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") { 

            if (interactionText.text == "Press E to open Door")
            {
                interactionText.text = "";
            }
         }
    }


    private void OnTriggerStay(Collider other) //Checks if player is inside the box collider with trigger
    {
        
        if (Input.GetKeyDown(KeyCode.E) && other.gameObject.tag == "Player")
        {
            if (moneyScript.totalMoney >= moneyRequired)
            {
                if (interactionText.text == "Press E to open Door")
                {
                    interactionText.text = "";
                }

                moneyScript.UnlockAcces(moneyRequired);

                gameObject.SetActive(false);

            }
        }
    }
}
