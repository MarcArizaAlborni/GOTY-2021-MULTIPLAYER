using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
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
}
