using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{

    public MoneyManager moneyScript;
    public int moneyRequired = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Press E to Open Door");
    }


    private void OnTriggerStay(Collider other) //Checks if player is inside the box collider with trigger
    {
        
        if (Input.GetKeyDown(KeyCode.E) && other.gameObject.tag == "Player")
        {
            if (moneyScript.totalMoney >= moneyRequired)
            {
                moneyScript.UnlockAcces(moneyRequired);

                gameObject.SetActive(false);

            }
        }
    }
}
