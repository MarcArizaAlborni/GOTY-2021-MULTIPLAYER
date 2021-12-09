using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        Keyboard kb = InputSystem.GetDevice<Keyboard>();
        if(kb.wKey.wasPressedThisFrame)
        {

        }
    }
}
