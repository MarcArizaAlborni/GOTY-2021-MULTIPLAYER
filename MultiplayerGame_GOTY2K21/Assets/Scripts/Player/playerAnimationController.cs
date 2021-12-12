using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    // Update is called once per frame
    void Update()
    {
        bool fwrdPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
  

        if (!isWalking && fwrdPressed)
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
        }
    }
}
