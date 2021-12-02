using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualHP_Enemy : MonoBehaviour
{

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
        if (isManaging == true) { //isManaging determines which part of the arm manages the total health of the arm (there are 3 parts for each arm)
                                  //(head has only 1 object, so will always have isManaging = true)

            currHP -= amountDamage;
      
            generalHP_script.UpdateGeneralHPEnemy(amountDamage);
        }
        else //If this object is not managing damage, give damage to managing object so that it applies
        {
            if (managingObject != null)
            {
                IndividualHP_Enemy manageScript = managingObject.GetComponent<IndividualHP_Enemy>();
                manageScript.ReceiveDamage(amountDamage);
            }
        }

    }


}
