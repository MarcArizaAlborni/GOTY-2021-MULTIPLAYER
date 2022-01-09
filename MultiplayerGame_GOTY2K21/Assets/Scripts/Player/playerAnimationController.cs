using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    Animator animator;
    TestingInput input;
    int isWalkingHash;
    int isRunningHash;
    int isShootingHash;
    int currentGunHash;

    Transform parentTransfrom;

    public Camera ourCamera;
    public float WalkingFov = 50.0f;


    // Start is called before the first frame update
    void Start()
    {
        parentTransfrom = transform.parent.transform;
        animator = GetComponent<Animator>();
        input = GetComponent<TestingInput>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isShootingHash = Animator.StringToHash("isShooting");
        currentGunHash = Animator.StringToHash("currentGun");
    }

    // Update is called once per frame
    void Update()
    {
        bool fwrdPressed = Input.GetKey("w");
        bool bwrdPressed = Input.GetKey("s");
        /*bool runPressed = Input.GetKey("left shift");
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);*/
        bool leftClickPressed = Input.GetMouseButton(0);
        bool rightClickPressed = Input.GetMouseButton(1);

        bool onePressed = Input.GetKey("1");
        bool twoPressed = Input.GetKey("2");

        bool isShooting = animator.GetBool(isShootingHash);

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        if(movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= input.characterSpeed * Time.deltaTime;
        }

        float rotX = ourCamera.transform.eulerAngles.x;
        float velZ = Vector3.Dot(movement.normalized, parentTransfrom.forward);
        float velX = Vector3.Dot(movement.normalized, parentTransfrom.right);

        animator.SetFloat("VelZ", -velZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelX", velX, 0.1f, Time.deltaTime);


        if (rotX >= 91.0f)
        {
            rotX = rotX - 360.0f;
            animator.SetFloat("aimVert", rotX, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("aimVert", rotX, 0.1f, Time.deltaTime);
        }

        if(onePressed)
        {
            animator.SetInteger(currentGunHash, 0);
        }
        if(twoPressed)
        {
            animator.SetInteger(currentGunHash, 1);
        }


      
    }

}
