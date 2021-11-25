
using UnityEngine;

public class GetHit : MonoBehaviour
{


    public float health = 25f;



    public void ReceiveDamage(float damageDone)
    {
        health = health - damageDone;


        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


}
