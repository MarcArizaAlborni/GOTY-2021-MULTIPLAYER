
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RifleScript : MonoBehaviour
{
    [Header("Gun Performance Management")]

    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 90;
    public float gunDamage=5f;
    public float gunRange=100f;

    [Header("Ammo Management")]
    private bool canShoot;
    private int currentAmmoMag;
    private int ammoInReserve;

    [Header("Hit Management")]
    public Camera ourCamera;

    [Header("Aim Management")]

    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSpeed = 10;

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
    public bool bulletRecoil = true;



    private void Start()
    {
        currentAmmoMag = clipSize;
        ammoInReserve = reservedAmmoCapacity;
        canShoot = true;
    }

    private void Update()
    {
       SetAim();
        

        if (Input.GetMouseButton(0) && canShoot && currentAmmoMag > 0)
        {
            canShoot = false;
            currentAmmoMag--;
            StartCoroutine(ShootGun());
        }
        else if (Input.GetKeyDown(KeyCode.R) && currentAmmoMag < clipSize && ammoInReserve > 0)
        {
            int amountNeeded = clipSize - currentAmmoMag;

            if (amountNeeded >= ammoInReserve)
            {
                currentAmmoMag += ammoInReserve;
                ammoInReserve -= amountNeeded;
            }
            else
            {
                currentAmmoMag = clipSize;
                ammoInReserve -= amountNeeded;
            }
           
        }
    }

    private void SetRotation()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseAxis *= mouseSensitivity;
        currentRotation += mouseAxis;

        currentRotation.y = Mathf.Clamp(currentRotation.y, -90, 90);

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;

        transform.root.localRotation = Quaternion.AngleAxis(currentRotation.x, Vector3.up);
        transform.parent.localRotation = Quaternion.AngleAxis(-currentRotation.y, Vector3.right);


    }
    private void SetAim()
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetMouseButton(1))
            target = aimingLocalPosition;

        Vector3 desiredPosition;
        desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * aimSpeed);


        transform.localPosition = desiredPosition;
    }

    private void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * 0.1f;

        if (randomizeRecoil)
        {
            float xRecoil = UnityEngine.Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = UnityEngine.Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(xRecoil, yRecoil);
            currentRotation += recoil;
        }
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
        if (Physics.Raycast(ourCamera.transform.position, ourCamera.transform.forward, out hit, gunRange))
        {
            Debug.Log(hit.transform.name);

           
            ManageTargetHP(hit);
            ManageBullet(hit.point);
            

        }
        else
        {
            Ray ray = ourCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            ManageBullet(ray.GetPoint(gunRange));
        }
    }
    
    void ManageBullet(Vector3 hitPosition)
    {
        GameObject projObj;
        if (bulletRecoil == false)
        {
            projObj = Instantiate(bulletPrefab, bulletSpawnPos.transform.position, Quaternion.identity);
        }
        else
        {
            float Xvar=(float) Math.Round(UnityEngine.Random.Range(-0.1f, 0.1f), 3) ;
            float Yvar =(float) Math.Round(UnityEngine.Random.Range(-0.1f, 0.1f), 3) ;

            Vector3 spawnPoint = new Vector3(bulletSpawnPos.transform.position.x+Xvar , bulletSpawnPos.transform.position.y +Yvar, bulletSpawnPos.transform.position.z);
            projObj = Instantiate(bulletPrefab, spawnPoint, Quaternion.identity);
           // Debug.Log("X:" + Xvar);
           // Debug.Log("Y:"+Yvar);
        }

        projObj.GetComponent<Rigidbody>().velocity=(hitPosition-bulletSpawnPos.transform.position).normalized * projectileSpeed;
    }

    void ManageTargetHP(RaycastHit hit)
    {
        ManagerHP_Enemy hpManagerScript = hit.transform.GetComponent<ManagerHP_Enemy>();


        if (hpManagerScript != null)
        {

            hpManagerScript.UpdateGeneralHPEnemy(gunDamage);


        }


        IndividualHP_Enemy hpIndividualScript = hit.transform.GetComponent<IndividualHP_Enemy>();


        if (hpIndividualScript != null)
        {

            hpIndividualScript.ReceiveDamage(gunDamage);

        }
    }

}