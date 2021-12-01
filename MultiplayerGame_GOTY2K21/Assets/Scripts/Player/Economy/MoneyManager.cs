using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{

    public RifleScript rifleScript;
    public PistolScript pistolScript;
    public ChungerWafferScript chungerWafferScript;
    


    [HideInInspector] public int totalMoney = 0;

    void Start()
    {
        
    }


    void RemoveMoneyAmount(int moneyRemoved)
    {


        totalMoney -= moneyRemoved;

    }


    public void UnlockAcces(int moneyRequired)
    {

        RemoveMoneyAmount(moneyRequired);


    }

    public void ObtainAmmo(int moneyRequired ,float ammoAdded)
    {

        RemoveMoneyAmount(moneyRequired);


        //Add Ammo




    }

    


 
}
