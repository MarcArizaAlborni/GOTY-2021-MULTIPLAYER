using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualHP_Enemy : MonoBehaviour
{

    public GameObject objectDamaged;
    public float maxHP;
    [HideInInspector] public float currHP;

    public ManagerHP_Enemy generalHP_script;

    public bool isManaging = false;

    public GameObject managingObject;

    private void Start()
    {
        currHP = maxHP;
    }


    

    public void ReceiveDamage(float amountDamage)
    {
        if (isManaging == true) { 

            currHP -= amountDamage;
      
            generalHP_script.UpdateGeneralHPEnemy(amountDamage);
        }
        else
        {
            if (managingObject != null)
            {
                IndividualHP_Enemy manageScript = managingObject.GetComponent<IndividualHP_Enemy>();
                manageScript.ReceiveDamage(amountDamage);
            }
        }

    }


}
