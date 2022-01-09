using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Id_zombieGenerator : MonoBehaviour
{

   [HideInInspector]public int id;



    void Start()
    {

       GameObject manager= GameObject.Find("Manager");


        WaveManagerScript waveScript = manager.GetComponent<WaveManagerScript>();


        id = Random.Range(100, 100000);
        // while (!GenerateID(waveScript))
        //{
        //      
        //}


        Debug.Log(id);
      




    }



    bool GenerateID(WaveManagerScript script)
    {
       id= Random.Range(100, 500);

        for (int i = 0; i < script.activeZombiesList.Count; ++i)
        {

            Id_zombieGenerator idScript= script.activeZombiesList[i].GetComponent<Id_zombieGenerator>();

            if (idScript.gameObject != script.activeZombiesList[i])
            {

                if (idScript.id == id)
                {
                    return true;
                }
            }

        }



        return true;

    }

   
}
