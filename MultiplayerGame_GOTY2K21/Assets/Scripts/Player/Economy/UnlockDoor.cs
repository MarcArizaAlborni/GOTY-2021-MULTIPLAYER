using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{

    public MoneyManager moneyScript;
    public int moneyRequired;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            moneyScript.UnlockAcces(moneyRequired);

            //ADD AMMO
            gameObject.SetActive(false);

        }
    }
}
