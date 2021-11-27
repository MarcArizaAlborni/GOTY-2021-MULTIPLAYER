
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


    public Vector3 addedRecoil;//Recoil
    public Vector3 initialRot; //Recoil
    

    private void Start()
    {
        Cursor.visible = false;
        initialRot = gameObject.transform.localEulerAngles;

        addedRecoil = new Vector3(-3f, 0f, 0f);
    }
    void Update()
    {

        internalTimer -= Time.deltaTime;


        if (Input.GetMouseButton(0) && internalTimer <= 0.0f)
        {
            Shooting();
        }
        else if (internalTimer <= 0.0f)
        {

        }
        else
        {
           // ResetRecoil();
        }
    }

    void SetRecoil()
    {
        gameObject.transform.localEulerAngles += addedRecoil;
    }

    void ResetRecoil()
    {
        gameObject.transform.localEulerAngles = initialRot;
    }

    void Shooting()
    {
        //SetRecoil();
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
