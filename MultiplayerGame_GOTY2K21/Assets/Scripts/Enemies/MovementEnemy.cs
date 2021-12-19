using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MovementEnemy : MonoBehaviour
{
    [Header("Time")]
    public float timeToSearch = 10.0f;
    private float timer = 0.0f;
    
    public Transform target;
    private NavMeshAgent agent;
    private Animator currentAnimation;
    // Start is called before the first frame update

    [HideInInspector] public bool attackingNow=false;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        currentAnimation = gameObject.GetComponent<Animator>();

        SteeringSeek(target.GetChild(0).transform.position);
        SetRunning();
    }
    // Update is called once per frame
    void Update()
    {
        SteeringSeek(target.GetChild(0).transform.position);
        float test = agent.remainingDistance;
        if (test <= agent.stoppingDistance)
        {
            //SetAttack();
            
        }
        else
        {
            SetRunning();

        }

       


        if (timer > timeToSearch)
        {
            SearchNewTarget();
            timer = 0;
        }
        timer += Time.smoothDeltaTime;
    }

    void SteeringSeek(Vector3 targetPos)
    {

        agent.SetDestination(targetPos);
      
    }


    //Search new target
    public void SearchNewTarget() 
    {
        GameObject[] goes = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject go in goes)
        {
            if (!go.GetComponent<PlayerHealthManager>().playerDead)
            {
                //If distance of current target is bigger than a new one, set this as new target
                if (Vector3.Distance(agent.transform.position, go.transform.position) < Vector3.Distance(agent.transform.position, target.transform.position))
                {
                    target = go.transform;
                }
            }
        }
    }

    //Animation functions
    public void SetAttack()
    {
        currentAnimation.SetBool("IsRunning", false);
        currentAnimation.SetBool("IsDead", false);
        currentAnimation.SetBool("IsAttacking", true);

        
        
    }

    public void SetDead()
    {
        currentAnimation.SetBool("IsRunning", false);
        currentAnimation.SetBool("IsDead", true);
        currentAnimation.SetBool("IsAttacking", false);
    }

    public void SetRunning()
    {
        currentAnimation.SetBool("IsRunning", true);
        currentAnimation.SetBool("IsDead", false);
        currentAnimation.SetBool("IsAttacking", false);
    }
}
