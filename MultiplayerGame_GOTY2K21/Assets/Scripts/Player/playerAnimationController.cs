using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool goForward = Input.GetKey("w");
        bool isWalking = animator.GetBool(isWalkingHash);

        if (!isWalking && goForward)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !goForward)
        {
            animator.SetBool(isWalkingHash, false);
        }
    }
}
