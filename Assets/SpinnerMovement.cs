using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerMovement : MonoBehaviour
{
   

    [Range(10,1000)] public float speed;
    public bool clockwise;

    int direction;
    Quaternion to;

    private void Start()
    {
        direction = clockwise ? 1 : -1;
    }

    void Update()
    { 
        to = Quaternion.Euler(Vector3.up * (transform.localRotation.eulerAngles.y + direction*speed * Time.deltaTime));  
        transform.localRotation = Quaternion.Slerp(transform.localRotation, to,1);
    }
}
