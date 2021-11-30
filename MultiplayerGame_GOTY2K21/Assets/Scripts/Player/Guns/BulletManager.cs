using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    public float lifeTime = 5.0f;


    private void Start()
    {
        StartCoroutine(DestroyBulletTime());
    }

     IEnumerator DestroyBulletTime()
     {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
     }


    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

}
