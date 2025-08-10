using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatAmplitude = 0.5f; 
    public float floatFrequency = 1f;   
    public float rotationSpeed = 50f;  

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; 
    }

    void Update()
    {
     
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

       
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
