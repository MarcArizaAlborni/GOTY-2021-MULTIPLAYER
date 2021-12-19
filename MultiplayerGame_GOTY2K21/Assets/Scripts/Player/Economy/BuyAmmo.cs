using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyAmmo : MonoBehaviour
{
    public MoneyManager moneyScript;

   

    public int moneyRequired = 0;
    //  public int ammoGiven = 0;


    public Text interactionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Press E to get Ammo");
        }

        interactionText.text = "Press E to buy Ammo";
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactionText.text == "Press E to buy Ammo")
        {
            interactionText.text = "";
        }
    }

    private void OnTriggerStay(Collider other) //Checks if player is inside the box collider with trigger
    {

       
        if ( other.gameObject.tag == "Player" )
        {




            if (Input.GetKeyDown(KeyCode.E))
            {


                if (moneyScript.totalMoney >= moneyRequired)
                {

                    if (interactionText.text == "Press E to buy Ammo")
                    {
                        interactionText.text = "";
                    }
                    moneyScript.ObtainAmmo(moneyRequired);
                }
            }
        }
    }

}


