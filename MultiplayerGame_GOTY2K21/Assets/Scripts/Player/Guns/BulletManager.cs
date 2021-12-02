using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    public float lifeTime = 5.0f;

    [Header("Collision Particles")]
    public bool wantCollisionParticles=false;
    public GameObject collisionParticleObject;





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
        if (collision.gameObject.tag != "Bullet")
        {

            if (collision.gameObject.tag == "Concrete")
            {
               GameObject particleObj = Instantiate(collisionParticleObject, gameObject.transform.position, Quaternion.identity);
                

                ParticleSystem particle = particleObj.GetComponent<ParticleSystem>();
                particle.Play();
            }





                Destroy(gameObject);
        }
    }

 
}
