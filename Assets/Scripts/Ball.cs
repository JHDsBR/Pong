using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    float leftLimit  = 0;
    float rightLimit = 0; 

    int collisionMask;

    List<Vector3> trajectoryPoints = new List<Vector3>();
    public LineRenderer lineRenderer;

    public Vector3 velocity;
    Vector3 currentVelocity;
    Vector3 direction;

    public float speed = 4;
    float minSpeed = 4;
    float maxSpeed = 20;

    float increaseSpeedRate = 0.1f;

    float maxAngleChange = 15;
    float minAngleChange = 10;

    bool started = false;

    [HideInInspector]
    public float ray, distanceFromWall, angle;

    void Start() 
    {
        ray = transform.localScale.z / 2;
        print("ray ->" + ray);
        leftLimit = -Camera.main.orthographicSize * Camera.main.aspect - transform.localScale.z / 2;
        rightLimit = Camera.main.orthographicSize * Camera.main.aspect + transform.localScale.z / 2;

        collisionMask = LayerMask.GetMask("Solid");
        ResetPosition();
    }

    public void Update() {

        speed += increaseSpeedRate * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        CheckCollision(velocity);
        // CheckCollision(Mathf.Sign(velocity.normalized.z) * Vector3.forward * (ray));
        // CheckCollision(Mathf.Sign(velocity.normalized.x) * Vector3.right * (ray));

        SimulateBallTrajectory();

        transform.position += velocity * Time.deltaTime;

        // if (transform.position.x <= leftLimit) {
        //     // OutLeft?.Invoke();
        //     ResetPosition();
        // }
        // if(transform.position.x >= rightLimit) {
        //     // OutRight?.Invoke();
        //     ResetPosition();
        // }

    }

    void CheckCollision(Vector3 directionToCheck) {

        RaycastHit hit;
        Physics.Raycast(transform.position, directionToCheck, out hit, ray, collisionMask);
        angle = Vector3.Angle(velocity, Vector3.right);
        // Debug.DrawRay(transform.position, directionToCheck, Color.red);
        if (hit.collider != null) {
            // print(1);
            velocity = Vector3.Reflect(velocity, hit.normal); // calcula a direção da bola após a colisão

            if (hit.collider.gameObject.tag == Tags.bar) {

                float direction = hit.collider.GetComponent<BarController>().GetDirectionInput();

                if (direction == 0) 
                    return;

                Vector3 newDirection = velocity;

                angle = Vector3.Angle(velocity, Vector3.right);

                var sign = Mathf.Sign(velocity.z);
                var angleChange = angle + direction * sign * 30;

                angleChange = Mathf.Clamp(angleChange, 10, 50);
                newDirection = new Vector3(Mathf.Cos(angleChange * Mathf.Deg2Rad),
                                            transform.position.y,
                                            Mathf.Sin(angleChange * Mathf.Deg2Rad));

                newDirection.z *= sign;

                velocity = newDirection.normalized * speed;
            }

        }
    }

    public void ResetPosition() {

       transform.position = Vector3.zero;

       float angle = UnityEngine.Random.Range(15, 45);

       direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),
                               transform.position.y,
                               Mathf.Sin(angle * Mathf.Deg2Rad));

       if (UnityEngine.Random.Range(0, 2) == 0) direction.x *= -1;
       if (UnityEngine.Random.Range(0, 2) == 0) direction.z *= -1;

       speed = minSpeed;
       velocity = direction * speed;
            
    }

    public void FreezeBall() {
        currentVelocity = velocity;
        velocity        = Vector2.zero;
        transform.position = Vector3.up * 1000;
    }

    public void FreeBall() {
        velocity = currentVelocity;
    }

    void SimulateBallTrajectory() 
    {

        
        List<Vector3> points = new List<Vector3>(){transform.position};

        RaycastHit hit;
        Vector3 lastPos = transform.position;
        Vector3 lastDir = velocity;
        int interactions = 6;

        for (var i = 0; i < interactions; i++)
        {
            Physics.Raycast(lastPos, lastDir, out hit, Mathf.Infinity, collisionMask);
            
            if(hit.collider == null)
                break;

            float offset = 0;

            if(Mathf.Approximately(Mathf.Abs(hit.normal.z), 1f))
            {
                print("z "+hit.normal);
                offset = (ray) / lastDir.normalized.z * -hit.normal.z;
            }
            else if(Mathf.Approximately(Mathf.Abs(hit.normal.x), 1f))
            {
                print("x");
                offset = (ray) / lastDir.normalized.x * -hit.normal.x;
            }

            // hit.point = new Vector3(hit.point.x - (ray) * hit.normal.x, hit.point.y, hit.point.z + (ray) * hit.normal.z);
            print("end -> "+hit.point);
            print("hit -> "+hit.normal);
            print("dir -> "+lastDir.normalized);
            // print(hit.normal);
            print("offset -> "+offset);
            // Debug.DrawLine(transform.position, hit.point, UnityEngine.Color.green);
            hit.point = hit.point - offset * lastDir.normalized;
            print("start -> "+hit.point);

            points.Add(hit.point);

            // if(hit.point.x > transform.position.x)
            //     break;

            lastDir = Vector3.Reflect(lastDir, hit.normal);
            lastPos = hit.point;
        }

        lineRenderer.positionCount = points.Count;
        int idx=0;

        foreach (Vector3 point in points)
        {
            lineRenderer.SetPosition(idx, point);
            // targetPosition = point.z;
            idx++;
        }
    }

    // public float speed; // velocidade que a bola se move
    // private Vector3 myDirection; // em que direção a bola está indo
    // [SerializeField] private Rigidbody myRb;

    // void OnDrawGizmosSelected()
    // {
 
    // #if UNITY_EDITOR
    //     Gizmos.color = Color.red;
 
    //     Gizmos.DrawLine(
    //         transform.position,
    //         transform.position + myDirection * 4
    //     );
 
    // #endif
    // }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     myDirection = GetRandomDirection();
    //     myRb.velocity = myDirection * speed;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     myRb.velocity = myDirection * speed;
    //     // Movement();
    // }

    // void Movement()
    // {
    //     transform.position += myDirection * speed * Time.deltaTime;
    // }

    // Vector3 GetRandomDirection()
    // {
    //     float myY = transform.position.y;
    //     List<Vector3> dirs = new List<Vector3>(){
    //         new Vector3(1,myY,1),
    //         new Vector3(1,myY,-1),
    //         new Vector3(-1,myY,1),
    //         new Vector3(-1,myY,-1)
    //     };

    //     return dirs[UnityEngine.Random.Range(0, dirs.Count)];
    // }

    // void OnCollisionEnter(Collision collision)
    // {
    //     // print(collision.contacts[0].normal);
    //     Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.red, 5f);
    //     BounceDirection(collision.contacts[0].normal);

    // }

    // void BounceDirection(Vector3 contactPosition)
    // {
    //     float cX = contactPosition.x;
    //     float cZ = contactPosition.z;
        
    //     float newX = myDirection.x, newZ = myDirection.z;

    //     if(cX != 0) // colidiu na horizontal
    //     {
    //         print("colidiu na horizontal");
    //         newX = -newX;
    //     }
    //     else if(cZ != 0) // colidiu na vertical
    //     {
    //         print("colidiu na vertical");
    //         newZ = -newZ;
    //     }
        
    //     myDirection = new Vector3(newX, myDirection.y, newZ);

    //     // ou
    //     // myDirection = new Vector3(cX != 0 ? -newX:newX, myDirection.y, cZ != 0 ? -newZ:newZ);

    //     myRb.velocity = myDirection * speed;
    //     // print(contactPosition);
    // }

    
}
