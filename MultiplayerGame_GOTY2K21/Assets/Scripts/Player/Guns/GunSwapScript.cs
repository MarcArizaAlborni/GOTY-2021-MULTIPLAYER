using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwapScript : MonoBehaviour
{

   public  RifleScript rifleScript;
   public  PistolScript pistolScript;
   public  ChungerWafferScript chungerScript;
    public HUDmanagerScript hudScript;

   public GameObject rifle;
   public GameObject pistol;
   public GameObject chunger;

    private void Start()
    {
        rifle.SetActive(true);
        pistol.SetActive(false);
        chunger.SetActive(false);
        GameObject.Find("PistolPanel").SetActive(false);
        GameObject.Find("RiflePanel").SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetMouseButton(1))
        {

            GameObject.Find("PistolPanel").SetActive(true);
            GameObject.Find("RiflePanel").SetActive(false);
            SetCanShootTrue();
                rifle.SetActive(false);
                pistol.SetActive(true);
            chunger.SetActive(false);
            
           
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetMouseButton(1))
        {

            hudScript.panelPistol.SetActive(false);
            hudScript.panelRifle.SetActive(true);
            SetCanShootTrue();
                rifle.SetActive(true);
                pistol.SetActive(false);
               chunger.SetActive(false);


        }

       // if (Input.GetKeyDown(KeyCode.Alpha3) && !Input.GetMouseButton(1))
       // {
       //
       //     SetCanShootTrue();
       //         rifle.SetActive(false);
       //         pistol.SetActive(false);
       //     chunger.SetActive(true);
       //
       // }
    }

    void SetCanShootTrue()
    {
        rifleScript.canShoot = true;
        pistolScript.canShoot = true;
        chungerScript.canShoot = true;
    }
}
