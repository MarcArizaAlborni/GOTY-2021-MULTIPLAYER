using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PistolScript : MonoBehaviour
{
    [Header("Gun Performance Management")]

    public float fireRate = 0.1f;
   
    public float gunDamage = 5f;
    public float gunRange = 100f;


    [Header("Ammo Management")]
    public bool canShoot;
    public int currentAmmoInMag; //Remaining ammo outside of mag
    public int ammoInReserve;
    public  int magSize = 7;
    public  int maxTotalAmmo = 32;

    [Header("Hit Management")]
    public Camera ourCamera;

    [Header("Aim Management")]

    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSpeed = 10;

    public bool removeCrosshairAim = true;
    public GameObject crosshairImage;

    public GameObject BulletAim;


    [Header("Mouse Settings")]
    public float mouseSensitivity = 1;
    private Vector2 currentRotation;
    public float weaponSwayAmount = 10;

    [Header("Recoil Management")]
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;

    [Header("Visual / Projectile Management")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPos;
    public float projectileSpeed = 2;

    [Header("Dead Player")]
    public PlayerHealthManager healthPlayerScript;

    private void Start()
    {
        currentAmmoInMag = magSize;
        ammoInReserve = maxTotalAmmo;
        ammoInReserve -= currentAmmoInMag;
        canShoot = true;

        BulletAim.transform.localPosition = new Vector3(0f, 0f, gunRange);
    }

    private void Update()
    {
        SetAim();


        if (Input.GetMouseButtonDown(0) && canShoot && currentAmmoInMag > 0 && !healthPlayerScript.playerDead)
        {
            //Debug.Log("CurrentMag" + currentAmmoInMag);
            //Debug.Log("InReserve" + ammoInReserve);
            canShoot = false;
            currentAmmoInMag--;
            StartCoroutine(ShootGun());
        }
        else if (Input.GetKeyDown(KeyCode.R) && currentAmmoInMag < magSize && ammoInReserve > 0 && !healthPlayerScript.playerDead)
        {
            int amountToReload = magSize - currentAmmoInMag;

            if (amountToReload <= ammoInReserve)
            {
                currentAmmoInMag += amountToReload;
                ammoInReserve -= amountToReload;
            }
            else
            {

                currentAmmoInMag += ammoInReserve;
                ammoInReserve -= ammoInReserve;

            }

        }
    }


    private void SetAim()//There are 2 positions for each of the guns (aiming , regular)
    {//When aim button is pressed, move gun from regular position to aiming

        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
        {
           // target = aimingLocalPosition;
           // if (removeCrosshairAim == true)
           // {
           //     crosshairImage.SetActive(false);
           // }
        }
        else
        {
            crosshairImage.SetActive(true);
        }

        Vector3 desiredPosition;
        desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSpeed);


        transform.localPosition = desiredPosition;
    }

    private void DetermineRecoil()//Only visual
    {
        transform.localPosition -= Vector3.forward * 0.2f;

      
    }

    private IEnumerator ShootGun()
    {
        DetermineRecoil();
        muzzleFlash.Play();

        CheckForHit();

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    void CheckForHit()
    {
      
        
            RaycastHit hit;

        if (Physics.Raycast(ourCamera.transform.position, ourCamera.transform.forward, out hit, gunRange)) //Send raycast from center of the camera  (where the crosshair is)
        {
            if (hit.collider.tag != "Bullet")
            {

                Debug.Log(hit.transform.name);
            }


            ManageTargetHP(hit);
                ManageBullet(hit.point);


        }
        else //If the raycast doesnt hit anything, set a point for the bullets to go to
        {
           // Ray ray = ourCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            ManageBullet(BulletAim.transform.position);
        }
        
    }

    void ManageBullet(Vector3 hitPosition)
    {
        Vector3 spawnPoint = bulletSpawnPos.transform.position;

        GameObject projObj = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, Quaternion.identity);

        projObj.GetComponent<Rigidbody>().velocity = (hitPosition - spawnPoint).normalized * projectileSpeed;
    }

    void ManageTargetHP(RaycastHit hit)
    {
        ManagerHP_Enemy hpManagerScript = hit.transform.GetComponent<ManagerHP_Enemy>();


        if (hpManagerScript != null)//only if hitting body (only body has the general hp manager script)
        {

            hpManagerScript.UpdateGeneralHPEnemy(gunDamage);

            return;
        }


        IndividualHP_Enemy hpIndividualScript = hit.transform.GetComponent<IndividualHP_Enemy>();


        if (hpIndividualScript != null)//only if hitting arms or head
        {

            hpIndividualScript.ReceiveDamage(gunDamage);

        }
    }

   

    public void AddAmmo()
    {
        ammoInReserve = maxTotalAmmo;
    }


}