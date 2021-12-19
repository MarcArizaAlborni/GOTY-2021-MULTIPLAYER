using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Text loginText;
    public Text placeholder;
    public GameObject panelLogin;
    public GameObject panelMainmenu;

   public void ButtonInteraction(bool connect)
   {


        if (connect)
        {
            SceneManager.LoadScene("LobyScene");
        }
        else
        {
            Application.Quit();
        }



   }

    public void LoginCheck()
    {
        if(!placeholder.text.Equals("Enter text...") && placeholder.text.Length > 0 )
        {
            panelLogin.SetActive(false);
            panelMainmenu.SetActive(true);
        }
        else if(loginText.text.Length > 0)
        {
            panelLogin.SetActive(false);
            panelMainmenu.SetActive(true);
        }
    }
}
