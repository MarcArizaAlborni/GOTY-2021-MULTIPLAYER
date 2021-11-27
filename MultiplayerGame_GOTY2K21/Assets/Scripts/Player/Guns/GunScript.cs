
using UnityEngine;
//fucking babies with gitignore
public class GunScript : MonoBehaviour
{

    public float damage = 5f;
    public float range = 100f;
    float internalTimer = 0.3f;
    public const float rateofFire = 0.2f;


    public Camera ourCamera;

    public ParticleSystem muzzleFlash;

    private void Start()
    {
        Cursor.visible = false;
    }
    void Update()
    {

        internalTimer -= Time.deltaTime;



        if (Input.GetMouseButton(0) && internalTimer <= 0.0f)
        {
            Shooting();
        }
    }

    void Shooting()
    {
        internalTimer = rateofFire;
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
