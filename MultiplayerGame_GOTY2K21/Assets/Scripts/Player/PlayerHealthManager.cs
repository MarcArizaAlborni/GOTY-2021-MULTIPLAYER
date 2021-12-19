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


    public float attackTime=1.0f;
    float currTime=0;

    public bool playerDead = false;


    public GameObject deadCollider;



    public float beenAttackedTime = 4.0f;
    float currNonAttackedTime = 0f;


    void Update()
    {
        if (attackTime < currTime && !playerDead)
        {

            for (int i = 0; i < wavesManager.activeZombiesList.Count; ++i)
            {
                MovementEnemy movScript = wavesManager.activeZombiesList[i].GetComponent<MovementEnemy>();


                if (movScript.attackingNow == true)
                {
                    currentPlayerHP -= damageDealtToPlayer;
                    movScript.attackingNow = false;
                    currNonAttackedTime = 0f;
                    if (currentPlayerHP <= 0)
                    {
                        playerDead = true;
                        deadCollider.SetActive(true);
                       
                    }

                }

            }

            currTime = 0f;
        }



        if (beenAttackedTime < currNonAttackedTime && !playerDead && currentPlayerHP < maxPlayerHP)
        {
            currentPlayerHP += 1;
        }


        currTime += Time.deltaTime;

        currNonAttackedTime += Time.deltaTime;

       


       


    }

    public void ReviveOtherPlayer()
    {

    }


    public void ReviveThisPlayer()
    {
        playerDead = false;
        currentPlayerHP = maxPlayerHP;
        deadCollider.SetActive(false);
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
        else if (other.tag == "DeadPlayer")
        {

            if( other.transform.parent.Find("Name").GetComponent<TextMesh>().text != gameObject.transform.Find("Name").GetComponent<TextMesh>().text)
            {
                if (Input.GetKeyDown(KeyCode.E)) {

                    other.transform.parent.GetComponent<PlayerHealthManager>().ReviveThisPlayer();
                }
            }


        }
    }


   


}
