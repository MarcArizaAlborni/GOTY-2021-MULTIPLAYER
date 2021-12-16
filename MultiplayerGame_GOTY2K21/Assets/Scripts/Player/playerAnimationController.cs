using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    Animator animator;
    TestingInput input;
    int isWalkingHash;
    int isRunningHash;

    Transform parentTransfrom;

    // Start is called before the first frame update
    void Start()
    {
        parentTransfrom = transform.parent.transform;
        animator = GetComponent<Animator>();
        input = GetComponent<TestingInput>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        /*bool fwrdPressed = Input.GetKey("w");
        bool bwrdPressed = Input.GetKey("s");
        bool runPressed = Input.GetKey("left shift");
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);*/
   
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

        if(movement.magnitude > 0)
        {
            movement.Normalize();
            movement *= input.characterSpeed * Time.deltaTime;

        }

        float velZ = Vector3.Dot(movement.normalized, parentTransfrom.forward);
        float velX = Vector3.Dot(movement.normalized, parentTransfrom.right);

      
        animator.SetFloat("VelZ", -velZ, 0.1f, Time.deltaTime);
        animator.SetFloat("VelX", velX, 0.1f, Time.deltaTime);
    






        /*if (!isWalking && fwrdPressed)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !fwrdPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if (!isRunning && (fwrdPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }

        if (isRunning && (!fwrdPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }*/
    }

}
