using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{

    public int maxPlayerHP;
    public int currentPlayerHP;

    public int damageDealtToPlayer;

    bool damagedRecently = false;
    bool atMaxHP = true;

    public CameraShake cameraShakeScript;
    public Camera ourCamera;

    public WaveManagerScript wavesManager;


    public float attackTime=0.5f;
    float currTime=0;


   
    void Update()
    {


        

        if (attackTime < currTime)
        {

            for (int i = 0; i < wavesManager.activeZombiesList.Count; ++i)
            {
                MovementEnemy movScript = wavesManager.activeZombiesList[i].GetComponent<MovementEnemy>();


                if (movScript.attackingNow == true)
                {
                    currentPlayerHP -= damageDealtToPlayer;
                    movScript.attackingNow = false;


                }

            }
            currTime = 0;
        }


        currTime += Time.deltaTime;
    }

   

    private void OnTriggerStay(Collider other)
    {
      

        if (other.tag == "ZombieAttack")
        {
           
            GameObject parentObj = other.transform.parent.gameObject;
            MovementEnemy movScript= parentObj.GetComponent<MovementEnemy>();
            movScript.SetAttack();
            movScript.attackingNow = true;
            Animator animatorZ= parentObj.GetComponent<Animator>();
        
           
        }
    }


    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "ZombieAttack")
    //    {
    //
    //        GameObject parentObj = other.transform.parent.gameObject;
    //        MovementEnemy movScript = parentObj.GetComponent<MovementEnemy>();
    //
    //        movScript.SetRunning();
    //        movScript.attackingNow = false;
    //
    //
    //
    //
    //    }
    //}


}
