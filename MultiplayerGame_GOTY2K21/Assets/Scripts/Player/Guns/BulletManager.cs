using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    public float lifeTime = 5.0f;

    [Header("Collision Particles")]
    public bool wantCollisionParticles=false;
    public GameObject collisionParticleObject;

    public RifleScript rifleScript;
    public PistolScript pistolScript;
    public ChungerWafferScript chungerWafferScript;

    private void Start()
    {
        StartCoroutine(DestroyBulletTime());




    }

     IEnumerator DestroyBulletTime() //After time has passed, destroy bullet
     {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
     }


    private void OnCollisionEnter(Collision collision) //If bullet collides with something, destroy bullet
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag!="Player")
        {

            if (collision.gameObject.tag == "Concrete")
            {
                if (collisionParticleObject != null)
                {
                    GameObject particleObj = Instantiate(collisionParticleObject, gameObject.transform.position, Quaternion.identity);


                    ParticleSystem particle = particleObj.GetComponent<ParticleSystem>();
                    particle.Play();
                }
            }
            else if (collision.gameObject.tag == "Zombie")
            {
              ZombieHitCollision(collision);

                Destroy(gameObject);

            }




            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }


        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

    void ZombieHitCollision(Collision collider)
    {

        float damage=0;

        if (pistolScript != null)
        {
            damage = pistolScript.gunDamage;
        }
        else if (rifleScript != null)
        {
            damage = rifleScript.gunDamage;
        }
        else if (chungerWafferScript != null)
        {
            damage = chungerWafferScript.gunDamage;
        }

        ManagerHP_Enemy hpManagerScript = collider.transform.GetComponent<ManagerHP_Enemy>();
        IndividualHP_Enemy hpIndividualScript = collider.transform.GetComponent<IndividualHP_Enemy>();

        if (hpManagerScript != null)//only if hitting body (only body has the general hp manager script)
        {
            
            Debug.Log(collider.transform.name);
            hpManagerScript.UpdateGeneralHPEnemy(damage);

            return;
        }
        else if (hpIndividualScript != null)//only if hitting arms or head
        {
            Debug.Log(collider.transform.name);
            hpIndividualScript.ReceiveDamage(damage);

        }






       




    }

 
}
