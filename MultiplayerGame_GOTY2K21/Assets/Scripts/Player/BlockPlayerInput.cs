using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlayerInput : MonoBehaviour
{

    float currTime = 0.0f;
    float timeToWait = 5.0f;


    private void Start()
    {
        gameObject.GetComponent<TestingInput>().enabled = false;
    }

    private void Update()
    {



        if (currTime >= timeToWait)
        {


            gameObject.GetComponent<TestingInput>().enabled = true;


        }

        currTime += Time.deltaTime;




    }

}
