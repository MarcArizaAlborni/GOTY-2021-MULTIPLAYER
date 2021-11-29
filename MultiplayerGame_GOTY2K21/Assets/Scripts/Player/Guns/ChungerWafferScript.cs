using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChungerWafferScript : MonoBehaviour
{
    [Header("Gun Performance Management")]

    public float fireRate = 0.1f;
    public int clipSize = 30;
    public int reservedAmmoCapacity = 90;
    public float gunDamage = 5f;
    public float gunRange = 100f;

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

    [Header("Visual Management")]
    public ParticleSystem muzzleFlash;

    private void Start()
    {
        currentAmmoMag = clipSize;
        ammoInReserve = reservedAmmoCapacity;
        canShoot = true;
    }

    private void Update()
    {
        SetAim();


        if (Input.GetMouseButtonDown(0) && canShoot && currentAmmoMag > 0)
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
            float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

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

            GetHit enemyReceiveDamage = hit.transform.GetComponent<GetHit>();


            if (enemyReceiveDamage != null)
            {
                enemyReceiveDamage.ReceiveDamage(gunDamage);
            }

        }
    }


}