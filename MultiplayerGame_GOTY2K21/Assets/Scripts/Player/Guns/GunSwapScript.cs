using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwapScript : MonoBehaviour
{


   public  RifleScript rifleScript;
   public  PistolScript pistolScript;

   public GameObject rifle;
   public GameObject pistol;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetMouseButton(1))
        {
            if (rifle.activeSelf)
            {
                rifle.SetActive(false);
                pistol.SetActive(true);
            }
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetMouseButton(1))
        {
            if (pistol.activeSelf)
            {
                rifle.SetActive(true);
                pistol.SetActive(false);
            }

        }


    }
}
