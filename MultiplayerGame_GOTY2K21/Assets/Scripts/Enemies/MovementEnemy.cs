using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MovementEnemy : MonoBehaviour
{
    [Header("Seek Parameters")]
    public float stopDistance = 2.0f;
    public float slowDistance = 4.0f;
    public float maxTurnSpeed = 5.0f;
    public float maxSpeed = 5.0f;
    public float acceleration = 2.0f;
    public float turnAcceleration = 2.0f;

    //Private variables for using SteeringSeek
    private float turnSpeed;
    private float movSpeed;
    private Quaternion rotation;
    private Vector3 movement;
    private Vector3 position;
    private Quaternion rot;
    private float timer = 0;

    public Transform target;
    private NavMeshAgent agent;
    private Animator currentAnimation;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        currentAnimation = gameObject.GetComponent<Animator>();

        position = gameObject.transform.position;

        SteeringSeek(target.GetChild(0).transform.position);
        SetRunning();
    }
    // Update is called once per frame
    void Update()
    {
        SteeringSeek(target.GetChild(0).transform.position);
       // Debug.Log("Agent Position: " + agent.transform.position);
       // Debug.Log("Position: " + position);
    }

    void SteeringSeek(Vector3 targetPos)
    {
        if (Vector3.Distance(targetPos, position) < agent.stoppingDistance)
        {
            SetAttack();
            return;
        }

        //seek delay so it does not iterate every frame
        if (timer > 0.5)
        {
            Seek(targetPos);
        }


        turnSpeed += turnAcceleration * Time.deltaTime;
        turnSpeed = Mathf.Min(turnSpeed, maxTurnSpeed);

        if (Vector3.Distance(targetPos, position) < agent.stoppingDistance)
        {
            //slows movement when arriving destination
            movSpeed = (maxSpeed * Vector3.Distance(targetPos, position)) / slowDistance;
        }
        else
        {
            SetRunning();
            movSpeed += acceleration * Time.deltaTime;
            movSpeed = Mathf.Min(movSpeed, maxSpeed);
            
        }
        rot = Quaternion.Slerp(rot, rotation, Time.deltaTime * turnSpeed);
        position += transform.forward.normalized * movSpeed * Time.deltaTime;

        if (timer > 0.5)
        {
            agent.destination = position;
            timer = 0;
        }
        transform.rotation = rot;

        timer += Time.smoothDeltaTime;
    }

    void Seek(Vector3 targetPos)
    {
        Vector3 direction = targetPos - position;
        direction.y = 0f;
        movement = direction.normalized * acceleration;
        float angle = Mathf.Rad2Deg * Mathf.Atan2(movement.x, movement.z);
        rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }


    //Animation functions
    void SetAttack()
    {
        currentAnimation.SetBool("IsRunning", false);
        currentAnimation.SetBool("IsDead", false);
        currentAnimation.SetBool("IsAttacking", true);
    }

    void SetDead()
    {
        currentAnimation.SetBool("IsRunning", false);
        currentAnimation.SetBool("IsDead", true);
        currentAnimation.SetBool("IsAttacking", false);
    }

    void SetRunning()
    {
        currentAnimation.SetBool("IsRunning", true);
        currentAnimation.SetBool("IsDead", false);
        currentAnimation.SetBool("IsAttacking", false);
    }
}
