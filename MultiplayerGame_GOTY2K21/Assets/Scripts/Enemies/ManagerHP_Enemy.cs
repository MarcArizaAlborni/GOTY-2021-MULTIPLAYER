using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerHP_Enemy : MonoBehaviour
{

    [Header("HEAD")]
    public IndividualHP_Enemy head_script;
    public GameObject head_obj;
    [Header("ARM R")]
     public IndividualHP_Enemy arm_R_script;
    //BONES
     public GameObject arm_R_obj;
     public GameObject handTop_R_obj;
     public GameObject midArm_R_obj;
    //VISUAL
    public GameObject varm_R_obj;
    public GameObject vhandTop_R_obj;
    public GameObject vmidArm_R_obj;

    private bool armRkilled = false;

    [Header("ARM L")]
    public IndividualHP_Enemy arm_L_script;
    //BONES
    public GameObject arm_L_obj;
    public GameObject handTop_L_obj;
    public GameObject midArm_L_obj;
    //VISUAL
    public GameObject varm_L_obj;
    public GameObject vhandTop_L_obj;
    public GameObject vmidArm_L_obj;

    private bool armLkilled = false;

    [Header("BODY")]

    private bool headKilled = false;
    public GameObject body_obj;


    public float maxGeneralHP;
    float currHPCount;

   
    private void Start()
    {
        currHPCount = maxGeneralHP;
    }

    private void Update()
    {
        if (head_script.currHP <= 0 && headKilled==false) //Check if the HP of the head is = 0
        {
            headKilled = true;
            Destroy(head_obj);
        }

        if (arm_L_script.currHP <= 0 && armLkilled==false) //Check if the HP of the left arm is = 0
        {

            arm_L_obj.SetActive(false);
            handTop_L_obj.SetActive(false);
            midArm_L_obj.SetActive(false);
            armLkilled = true;


            //we destroy visual obj
            Destroy(varm_L_obj);
            Destroy(vhandTop_L_obj);
            Destroy(vmidArm_L_obj);
        }

        if (arm_R_script.currHP <= 0&&armRkilled==false) //Check if the HP of the right arm is = 0
        {
            //we disable bones
            arm_R_obj.SetActive(false);
            handTop_R_obj.SetActive(false);
            midArm_R_obj.SetActive(false);
            armRkilled = true;


            //we destroy visual obj
            Destroy(varm_R_obj);
            Destroy(vhandTop_R_obj);
            Destroy(vmidArm_R_obj);
        }


        if (currHPCount <= 0) //When the total amount of the zombie is = 0, destroy zombie
        {
            Destroy(body_obj);
        }
    }

    public void UpdateGeneralHPEnemy(float amountDamage) //Reduces total amount of HP
    {
        currHPCount -= amountDamage;
    }

    public void SetHPtoDifficulty()
    {
       
    }

}
