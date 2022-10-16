using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed; // velocidade que a bola se move
    private Vector3 myDirection; // em que direção a bola está indo
    [SerializeField] private Rigidbody myRb;

    void OnDrawGizmosSelected()
    {
 
    #if UNITY_EDITOR
        Gizmos.color = Color.red;
 
        Gizmos.DrawLine(
            transform.position,
            transform.position + myDirection * 4
        );
 
    #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        myDirection = GetRandomDirection();
        myRb.velocity = myDirection * speed;
    }

    // Update is called once per frame
    void Update()
    {
        myRb.velocity = myDirection * speed;
        // Movement();
    }

    void Movement()
    {
        transform.position += myDirection * speed * Time.deltaTime;
    }

    Vector3 GetRandomDirection()
    {
        float myY = transform.position.y;
        List<Vector3> dirs = new List<Vector3>(){
            new Vector3(1,myY,1),
            new Vector3(1,myY,-1),
            new Vector3(-1,myY,1),
            new Vector3(-1,myY,-1)
        };

        return dirs[UnityEngine.Random.Range(0, dirs.Count)];
    }

    void OnCollisionEnter(Collision collision)
    {
        // print(collision.contacts[0].normal);
        Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 5f);
        BounceDirection(collision.contacts[0].normal);

    }

    void BounceDirection(Vector3 contactPosition)
    {
        float cX = contactPosition.x;
        float cZ = contactPosition.z;
        
        float newX = myDirection.x, newZ = myDirection.z;

        if(cX != 0) // colidiu na horizontal
        {
            print("colidiu na horizontal");
            newX = -newX;
        }
        else if(cZ != 0) // colidiu na vertical
        {
            print("colidiu na vertical");
            newZ = -newZ;
        }
        
        myDirection = new Vector3(newX, myDirection.y, newZ);

        // ou
        // myDirection = new Vector3(cX != 0 ? -newX:newX, myDirection.y, cZ != 0 ? -newZ:newZ);

        myRb.velocity = myDirection * speed;
        // print(contactPosition);
    }
}
