using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{

    public RifleScript rifleScript;
    public PistolScript pistolScript;
    public ChungerWafferScript chungerWafferScript;
    


    [HideInInspector] public int totalMoney = 0;

   


    void RemoveMoneyAmount(int moneyRemoved) //Removes amount of money from total amount
    {


        totalMoney -= moneyRemoved;

    }


    public void UnlockAcces(int moneyRequired) //Opens door
    {

        RemoveMoneyAmount(moneyRequired);


    }

    public void ObtainAmmo(int moneyRequired ) //Gives ammo to each of the guns
    {

        RemoveMoneyAmount(moneyRequired);


        //Add Ammo

        pistolScript.AddAmmo();
        rifleScript.AddAmmo();
        chungerWafferScript.AddAmmo();


    }

    


 
}
