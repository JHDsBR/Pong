using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float movement = Input.GetAxis("Vertical");
        // print(movement);
        transform.position += movementSpeed * movementSpeed * Time.deltaTime * transform.forward * movement;
    }
}
