using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyAmmo : MonoBehaviour
{
    public MoneyManager moneyScript;

   

    public int moneyRequired = 0;
  //  public int ammoGiven = 0;


  

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Press E to get Ammo");
    }

    private void OnTriggerStay(Collider other)
    {
       

        if (Input.GetKeyDown(KeyCode.E) && other.gameObject.tag == "Player" )
        {
            moneyScript.ObtainAmmo(moneyRequired);

        }
    }

}


