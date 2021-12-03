using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MovementEnemy : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;
    private Animator currentAnimation;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        currentAnimation = gameObject.GetComponent<Animator>();

        agent.SetDestination(target.position);
        //play move
        currentAnimation.SetBool("IsRunning", true);
        currentAnimation.SetBool("IsIdle", false);
    }
    // Update is called once per frame
    void Update()
    {
        if (agent.transform.position == target.position)
        {
            currentAnimation.SetBool("IsIdle", true);  //play idle
            currentAnimation.SetBool("IsRunning", false);
        }


    }  
}
