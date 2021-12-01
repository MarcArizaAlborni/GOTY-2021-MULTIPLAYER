using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{

    public MoneyManager moneyScript;
    public int moneyRequired;

    private void OnTriggerStay(Collider other)
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
