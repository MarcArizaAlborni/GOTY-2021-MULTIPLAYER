using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDmanagerScript : MonoBehaviour
{

    

    [Header("Rifle")]
    public GameObject panelRifle;
    public Text ammoMagRifleText;
    public Text ammoReserveRifleText;
    public RifleScript rifleScript;
    [Header("Pistol")]
    public GameObject panelPistol;
    public Text ammoMagPistolText;
    public Text ammoReservePistolText;
    public PistolScript pistolScript;

    [Header("Money")]
    public Text moneyText;
    public MoneyManager moneyScript;

    [Header("Round")]
    public Text roundText;
    public WaveManagerScript waveScript;


    void Update()
    {

        ManageUI();

        
    }

    public void ManageUI()
    {

        UpdateNumbers();
       



    }

    void UpdateNumbers()
    {
        //RIFLE
        ammoMagRifleText.text = rifleScript.currentAmmoInMag.ToString();
        ammoReserveRifleText.text = rifleScript.ammoInReserve.ToString();
        //PISTOL
        ammoMagPistolText.text = pistolScript.currentAmmoInMag.ToString();
        ammoReservePistolText.text = pistolScript.ammoInReserve.ToString();


        //AMMO
        moneyText.text ="$" +moneyScript.totalMoney.ToString();


        //ROUNDS

        roundText.text = waveScript.currentRoundNum.ToString();
    }
    
}
