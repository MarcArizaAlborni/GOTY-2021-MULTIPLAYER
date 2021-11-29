using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwapScript : MonoBehaviour
{


   public  RifleScript rifleScript;
   public  PistolScript pistolScript;
    public ChungerWafferScript chungerScript;

   public GameObject rifle;
   public GameObject pistol;
    public GameObject chunger;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetMouseButton(1))
        {
           
            
                rifle.SetActive(false);
                pistol.SetActive(true);
            chunger.SetActive(false);
            
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetMouseButton(1))
        {
            
            
                rifle.SetActive(true);
                pistol.SetActive(false);
               chunger.SetActive(false);


        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !Input.GetMouseButton(1))
        {
           
            
                rifle.SetActive(false);
                pistol.SetActive(false);
            chunger.SetActive(true);


        }


    }
}
