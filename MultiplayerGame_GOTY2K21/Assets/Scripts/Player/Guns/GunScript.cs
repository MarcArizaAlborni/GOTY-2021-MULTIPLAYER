
using UnityEngine;
//fucking babies with gitignore
public class GunScript : MonoBehaviour
{

    public float damage = 5f;
    public float range = 100f;


    public Camera ourCamera;

    public ParticleSystem muzzleFlash;

    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
           
            Shooting();
        }


        
    }

    void Shooting()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(ourCamera.transform.position, ourCamera.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            GetHit enemyReceiveDamage=hit.transform.GetComponent<GetHit>();

         
            if (enemyReceiveDamage != null)
            {
                enemyReceiveDamage.ReceiveDamage(damage);
            }

        }
    }
}
