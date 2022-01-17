using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMovement : MonoBehaviour
{
    public Transform GetTransform()
    {
        return transform;
    }
    public void SetTransform(Vector3 pos, Vector3 rot)
    {
        //transform.SetPositionAndRotation(pos, Quaternion.Euler(rot));
        transform.position = pos;
        transform.rotation = Quaternion.Euler(rot);
    }
}
