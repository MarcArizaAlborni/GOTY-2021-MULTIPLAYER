using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    //Input Field
    public InputField field;
    public Text loginText;
    public Text placeholder;

    //Panels
    public GameObject panelLogin;
    public GameObject panelMainmenu;
    public Text clientName;
    public Text connectedIp;

    private void Update()
    {
        clientName.text = "Name: "+field.text;
        //Obtencio Ip
        //connectedIp.text = 
    }

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
        string validate = loginText.text;

        if (validate.Contains(" ") || validate.Contains(".") || validate.Contains(",") || validate.Contains("*") || validate.Contains("^") ||
              validate.Contains("@") || validate.Contains("<") || validate.Contains(">") || validate.Contains("|") || validate.Contains("\"") ||
              validate.Contains("/") || validate.Contains("!") || validate.Contains("?") || validate.Contains("¿") || validate.Contains("º") || validate.Contains("ª"))
        {
            placeholder.text = "Invalid name";
            field.text = "";
        }
        else if (field.text.Length > 0 && validate.Length > 0)
        {
            panelLogin.SetActive(false);
            panelMainmenu.SetActive(true);
        }
    }
}
