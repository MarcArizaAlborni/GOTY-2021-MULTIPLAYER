using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyAmmo : MonoBehaviour
{
    public MoneyManager moneyScript;

   

    public int moneyRequired = 0;
    public int ammoGiven = 0;




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            moneyScript.ObtainAmmo(moneyRequired,ammoGiven);

           


        }
    }

}


