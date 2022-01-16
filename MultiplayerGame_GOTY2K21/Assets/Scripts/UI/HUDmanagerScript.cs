using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public Text zombieNumber;
    public WaveManagerScript waveScript;

    

    [Header("Escape")]
    public GameObject escPanel;
    bool escActive = false;


    void Update()
    {

        ManageUI();

        
    }

    public void ManageUI()
    {

        UpdateNumbers();


        CheckESCInput();

    }

    void UpdateNumbers()
    {
        if (rifleScript != null)
        {
            //RIFLE
            ammoMagRifleText.text = rifleScript.currentAmmoInMag.ToString();
            ammoReserveRifleText.text = rifleScript.ammoInReserve.ToString();
        }

        if (pistolScript != null)
        {
            //PISTOL
            ammoMagPistolText.text = pistolScript.currentAmmoInMag.ToString();
            ammoReservePistolText.text = pistolScript.ammoInReserve.ToString();
        }


        if (moneyScript != null)
        {
            //AMMO
            moneyText.text = "$" + moneyScript.totalMoney.ToString();
        }


        //ROUNDS

        if (waveScript != null)
        {
            roundText.text = "Round: " + waveScript.currentRoundNum.ToString();
            zombieNumber.text = "Zombies: " + waveScript.activeZombiesList.Count.ToString();
        }
    }

    void CheckESCInput()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!escActive) {
                OpenEscapePanel();

            }
            else
            {
                CloseEscapePanel();
            }
        }

      

    }

    void OpenEscapePanel()
    {
        escActive = true;
        panelPistol.SetActive(false);
        panelRifle.SetActive(false);
        escPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseEscapePanel()
    {
        escActive = false;
        panelPistol.SetActive(true);
        panelRifle.SetActive(true);
        escPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    public void DisconnectGame(bool disconnect)
    {
        if (disconnect == true)
        {
           Application.Quit();
        }
        else
        {
            CloseEscapePanel();
        }

    }
    
}
