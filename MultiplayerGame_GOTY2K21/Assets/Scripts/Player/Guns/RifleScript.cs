
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RifleScript : MonoBehaviour
{
    [Header("Gun Performance Management")]

    public float fireRate = 0.1f;
    public float gunDamage=5f;
    public float gunRange=100f;

   public CameraShake cameraShakeScript;

    public bool wantCameraShake=true;


    [Header("Ammo Management")]
    [HideInInspector]public bool canShoot;
    private int currentAmmoInMag; //Remaining ammo outside of mag
    private int ammoInReserve;
    public  int magSize = 30; //Max ammo in single Mag
    public  int maxTotalAmmo = 1000; //Total Ammo

    [Header("Hit Management")]
    public Camera ourCamera;
   // public ParticleSystem collisionParticle;

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
    public float projectileSpeed = 20;
    public bool bulletRecoil = true;



    private void Start()
    {
        currentAmmoInMag = magSize;
      

        ammoInReserve = maxTotalAmmo;
        ammoInReserve -= currentAmmoInMag;
        canShoot = true;


        BulletAim.transform.localPosition = new Vector3(0f, -0.7f, gunRange);

    }

    private void Update()
    {

      

        SetAim();
        
        
        

        if (Input.GetMouseButton(0) && canShoot && currentAmmoInMag > 0)
        {
            //Debug.Log("CurrentMag"+currentAmmoInMag);
            //Debug.Log("InReserve" + ammoInReserve);
            if (cameraShakeScript.shaking != true && wantCameraShake == true)
            {
                cameraShakeScript.StartShake();
            }

            canShoot = false;
            currentAmmoInMag--;
            StartCoroutine(ShootGun());
        }
        else if (Input.GetKeyDown(KeyCode.R) && currentAmmoInMag < magSize && ammoInReserve > 0)
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
            target = aimingLocalPosition;
            if (removeCrosshairAim == true)
            {
                crosshairImage.SetActive(false);
            }
        }
        else
        {
            
           
                crosshairImage.SetActive(true);
            
        }

        Vector3 desiredPosition;
        desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSpeed);


        transform.localPosition = desiredPosition;

       
    }

    private void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * 0.1f;

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
               
                //Debug.Log(hit.transform.name);
            }


            // ManageTargetHP(hit);



            //ManageTargetHP(BulletAim.transform.position);
            ManageBullet(BulletAim.transform.position);
            

        }
        else //If the raycast doesnt hit anything, set a point for the bullets to go to
        {
           // Ray ray = ourCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            ManageBullet(BulletAim.transform.position);
        }
    }
    
    void ManageBullet(Vector3 hitPosition) // Create bullet and give it velocity
    {
        GameObject projObj;

        Vector3 spawnPoint= bulletSpawnPos.transform.position;

        if (bulletRecoil == false)
        {
            projObj = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, Quaternion.identity);
            projObj.GetComponent<Rigidbody>().velocity = projectileSpeed * bulletSpawnPos.transform.forward;
        }
        else //For rifle we create variation to the spawnpoint of each of the bullets (only visual)
        {
            float Xvar=(float) Math.Round(UnityEngine.Random.Range(-0.05f, 0.05f), 3) ;
            float Yvar =(float) Math.Round(UnityEngine.Random.Range(-0.05f, 0.05f), 3) ;

             spawnPoint = new Vector3(bulletSpawnPos.transform.position.x + Xvar , bulletSpawnPos.transform.position.y +Yvar, bulletSpawnPos.transform.position.z);
            projObj = Instantiate(bulletPrefab, spawnPoint, Quaternion.identity);
            projObj.GetComponent<Rigidbody>().velocity = (hitPosition - spawnPoint).normalized * projectileSpeed;

        }

        

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