using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Ball : MonoBehaviour
{

    public AudioSource sound, soundHit;
    public GameObject ballChild;
    public Animator myAnim, sombraAnim;

    float leftLimit  = 0;
    float rightLimit = 0; 
    float topLimit = 0; 
    float bottomLimit = 0; 

    int collisionMask, collisionMaskEvent;

    List<Vector3> trajectoryPoints = new List<Vector3>();
    public LineRenderer lineRenderer;

    public Vector3 velocity;
    Vector3 currentVelocity;
    Vector3 direction;

    float speed = 4;
    public float minSpeed = 2;
    public float maxSpeed = 10;

    float increaseSpeedRate = 0.2f;

    float maxAngleChange = 35;
    float minAngleChange = 25;

    bool started = false, stop = false;

    [HideInInspector]
    public float ray, distanceFromWall, angle;

    private BarController lastTouchedBall;

    // public Material myMaterial;

    void Start() 
    {
        sound.volume = GameManager.Instance.GetVolume();
        ray = transform.localScale.z / 2;
        print("ray ->" + ray);
        leftLimit = -Camera.main.orthographicSize * Camera.main.aspect - transform.localScale.z / 2;
        rightLimit = Camera.main.orthographicSize * Camera.main.aspect + transform.localScale.z / 2;
        // RaycastHit[] hit;
        collisionMask = LayerMask.GetMask("Solid");
        collisionMaskEvent = LayerMask.GetMask("Event");
        // Physics.RaycastAll(transform.position, Vector3.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Solid"));
        RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position, transform.right), Mathf.Infinity, collisionMask);

        foreach (RaycastHit hit in hits)
        {
            if(hit.collider != null && hit.collider.tag != Tags.bar)
            {
                rightLimit = hit.point.x - transform.localScale.z/1.5f;
                Debug.DrawLine(transform.position, hit.point, UnityEngine.Color.blue, 10f);
                break;
            }
        }

        hits = Physics.RaycastAll(new Ray(transform.position, -transform.right), Mathf.Infinity, collisionMask);

        foreach (RaycastHit hit in hits)
        {
            if(hit.collider != null && hit.collider.tag != Tags.bar)
            {
                leftLimit = hit.point.x + transform.localScale.z/1.5f;
                Debug.DrawLine(transform.position, hit.point, UnityEngine.Color.blue, 10f);
                break;
            }
        }

        RaycastHit h;

        Physics.Raycast(Vector3.zero, Vector3.forward, out h, Mathf.Infinity, collisionMask);
        topLimit = h.distance-ray+0.01f;

        Physics.Raycast(Vector3.zero, -Vector3.forward, out h, Mathf.Infinity, collisionMask);
        bottomLimit = -h.distance+ray-0.01f;

        Debug.DrawLine(Vector3.zero, Vector3.forward*topLimit, UnityEngine.Color.magenta, 10);
        Debug.DrawLine(Vector3.zero, Vector3.forward*bottomLimit, UnityEngine.Color.magenta, 10);

        // print(leftLimit);
        // print(rightLimit);

        GameplayManager.Instance.AddNewBall(this.gameObject);
        SetSpeed(GameplayManager.Instance.ballSpeed);
        ResetPosition();
    }


    public void Update() 
    {
        KeepBallInsideCamp();
        // speed += increaseSpeedRate * Time.deltaTime;
        // speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        // transform.position += velocity * Time.deltaTime;



    }

    void FixedUpdate()
    {
        if(GameplayManager.Instance.stopGame || stop)
            return;
        // speed += increaseSpeedRate * Time.deltaTime;
        // speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        // myMaterial.SetTextureOffset("_MainTex", new Vector2(-velocity.z, -velocity.x)*(speed/2));

        ballChild.transform.Rotate(new Vector3(-velocity.z, 0, -velocity.x)*(speed/2));
        // spriteMask.transform.rotation = new Quaternion(0,0,0,0);
        // CheckCollision(velocity * Time.fixedDeltaTime);
        // CheckCollision(Mathf.Sign(velocity.z) * Vector3.forward * (ray));
        // CheckCollision(Mathf.Sign(velocity.x) * Vector3.right * (ray));
        // transform.position += velocity * Time.fixedDeltaTime;
        // SimulateBallTrajectory();
        Move();
        // CheckCollisionWithPowerUp(transform.position, newPos);
        // transform.position = newPos;

    }

    // IEnumerator RotateToFace()
    // {
    //     while(true)
    //     {
    //         while(stop || GameplayManager.Instance.stopGame)
    //         {
    //             //wait
    //         }



    //         yield return null;
    //     }
    // }

    void KeepBallInsideCamp()
    {
        // return;
        if(transform.position.z >= topLimit)
        {
            print("Saiu por cima");
            velocity = Vector3.Reflect(velocity, Vector3.forward).normalized;
            transform.position = new Vector3(transform.position.x,transform.position.y,topLimit-0.01f);
        }
        else if(transform.position.z <= bottomLimit)
        {
            print("Saiu por baixo");
            velocity = Vector3.Reflect(velocity, -Vector3.forward).normalized;
            transform.position = new Vector3(transform.position.x,transform.position.y,bottomLimit+0.01f);
        }
    }

    void Move()
    {
        Vector3 step = speed * Time.fixedDeltaTime * velocity;
        Vector3 newPos = transform.position + step;
        BarController raquete = null;

        RaycastHit hit, realHit, oldRealHit;
        Physics.Raycast(transform.position, velocity, out hit, Mathf.Infinity, collisionMask);
        // Debug.DrawLine(transform.position, hit.point, UnityEngine.Color.magenta);

        if (hit.collider != null)
        {
            // if(hit.collider.tag == Tags.powerup)
            if(hit.collider.tag != Tags.bar && CheckIfPositionOutLimit(newPos)) // checa se a posição futura irá colider na parede atrás dos jogadores
            {
                soundHit.Play(0);
                return;
            }
            
            realHit = RealHit(hit, velocity);

            if((realHit.point-transform.position).magnitude <= step.magnitude)
            {
                if(hit.collider.tag == Tags.bar)
                    raquete = hit.collider.GetComponent<BarController>();

                print("Primeira colisão");
                sound.Play(); // toca o som
                CameraShaker.Instance.ShakeOnce(5f,.1f,.1f,.1f); // faz a câmera tremer
                // Debug.DrawLine(transform.position, realHit.point, UnityEngine.Color.yellow, 2f);
                velocity                        = Vector3.Reflect(velocity, hit.normal).normalized; // reflete a direção da colisão
                float distanceAfterCollision    = step.magnitude - (realHit.point-transform.position).magnitude; // calcula a distance depois da colisão
                newPos                          = realHit.point + velocity * distanceAfterCollision;

                // START NOVO

                Physics.Raycast(realHit.point, velocity, out hit, Mathf.Infinity, collisionMask);

                if (hit.collider != null)
                {
                    if(hit.collider.tag != Tags.bar && CheckIfPositionOutLimit(newPos)) // checa se a posição futura irá colider na parede atrás dos jogadores
                    {
                        soundHit.Play(0);
                        return;
                    }

                    oldRealHit = realHit;
                    realHit = RealHit(hit, velocity);
                    
                    if((realHit.point-transform.position).magnitude <= step.magnitude-distanceAfterCollision)
                    {

                        if(hit.collider.tag == Tags.bar)
                            raquete = hit.collider.GetComponent<BarController>();

                        print("Segunda colisão");
                        
                        sound.Play();
                        CameraShaker.Instance.ShakeOnce(5f,.1f,.1f,.1f);
                        velocity                = Vector3.Reflect(velocity, hit.normal).normalized;
                        distanceAfterCollision  = step.magnitude - distanceAfterCollision - (realHit.point-oldRealHit.point).magnitude;
                        newPos                  = realHit.point + velocity * distanceAfterCollision;
                    }
                }

                // newPos = realHit.point + velocity * distanceAfterCollision;
                // END NOVO

                // return;
            }

            if (raquete != null) {

                this.lastTouchedBall = raquete;
                print("Colisão com raquete");        
                GameplayManager.Instance.lastTouchedBall = raquete;

                float direction = raquete.GetDirectionInput();

                if (direction == 0) 
                    return;

                Vector3 newDirection = velocity;

                angle = Vector3.Angle(velocity, Vector3.right);

                var sign = Mathf.Sign(velocity.z);
                var angleChange = angle + direction * sign * 30;

                angleChange = Mathf.Clamp(angleChange, minAngleChange, maxAngleChange);
                newDirection = new Vector3(Mathf.Cos(angleChange * Mathf.Deg2Rad),
                                            transform.position.y,
                                            Mathf.Sin(angleChange * Mathf.Deg2Rad));

                newDirection.z *= sign;

                velocity = newDirection.normalized;
            }

        }

        CheckCollisionWithEvent(transform.position, newPos);

        transform.position = newPos;
    }

    void CheckCollisionWithEvent(Vector3 oldPos, Vector3 newPos)
    {
        RaycastHit hit;
        Vector3 distance = newPos-oldPos;
        Physics.Raycast(oldPos, distance.normalized, out hit, distance.magnitude, collisionMaskEvent);
        if(hit.collider != null)
            hit.collider.gameObject.GetComponent<Event>().Active(this.GetComponent<Ball>());
    }

    void CheckCollisionWithPowerUp(Vector3 origin, Vector3 end)
    {
        // Vector3 direction = end-origin;
        // RaycastHit hit;
        // Physics.Raycast(origin, direction, out hit, direction.magnitude, GameplayManager.Instance.powerUpMask);

        // if(hit.collider != null)
        //     hit.collider.GetComponent<PowerUp>().StartEffect();
    }


    bool CheckIfPositionOutLimit(Vector3 position)
    {
        // verifica se a bola chegou até o lado direito ou esquerdo

        if(!(position.x <= leftLimit || position.x >= rightLimit)) return false;

        if (position.x <= leftLimit) GameplayManager.Instance.HitLeft();
        else GameplayManager.Instance.HitRight();
        
        ResetPosition();
        
        return true;
    }

    void CheckCollision(Vector3 directionToCheck) {

        // int checks = 2;

        // for (var i = 0; i < checks; i++)
        // {
            
        // }

        RaycastHit hit;
        Physics.Raycast(transform.position, directionToCheck, out hit, directionToCheck.magnitude + ray, collisionMask);
        angle = Vector3.Angle(velocity, Vector3.right);
        // Debug.DrawRay(transform.position, directionToCheck, Color.red);
        if (hit.collider != null) {
            // print(1);
            velocity = Vector3.Reflect(velocity, hit.normal).normalized; // calcula a direção da bola após a colisão

            if (hit.collider.gameObject.tag == Tags.bar) {

                float direction = hit.collider.GetComponent<BarController>().GetDirectionInput();

                if (direction == 0) 
                    return;

                Vector3 newDirection = velocity;

                angle = Vector3.Angle(velocity, Vector3.right);

                var sign = Mathf.Sign(velocity.z);
                var angleChange = angle + direction * sign * 30;

                angleChange = Mathf.Clamp(angleChange, minAngleChange, maxAngleChange);
                newDirection = new Vector3(Mathf.Cos(angleChange * Mathf.Deg2Rad),
                                            transform.position.y,
                                            Mathf.Sin(angleChange * Mathf.Deg2Rad));

                newDirection.z *= sign;

                velocity = newDirection.normalized * speed;
            }

        }
    }

    public void ResetPosition() 
    {

        lastTouchedBall = null;
       transform.position = Vector3.zero;
    
       float angle = UnityEngine.Random.Range(minAngleChange, maxAngleChange);

       direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),
                               transform.position.y,
                               Mathf.Sin(angle * Mathf.Deg2Rad));

       if (UnityEngine.Random.Range(0, 2) == 0) direction.x *= -1;
       if (UnityEngine.Random.Range(0, 2) == 0) direction.z *= -1;

    //    speed = minSpeed;
    //    velocity = direction * speed;
       velocity = direction.normalized;
    //    velocity = new Vector3(-1,0,-0.505f).normalized;
        StartCoroutine(StopMovement(1.5f));     
    }

    public void FreezeBall() {
        currentVelocity = velocity;
        velocity        = Vector2.zero;
        transform.position = Vector3.up * 1000;
    }

    public void FreeBall() {
        velocity = currentVelocity;
    }


    RaycastHit RealHit(RaycastHit hit, Vector3 direction)
    {

        // verifica qual é a real colisão levando em conta o raio da bola

        float offset = 0;

        if(Mathf.Approximately(Mathf.Abs(hit.normal.z), 1f))
        {
            // print("z "+hit.normal);
            offset = (ray) / direction.normalized.z * -hit.normal.z;
        }
        else if(Mathf.Approximately(Mathf.Abs(hit.normal.x), 1f))
        {
            // print("x");
            offset = (ray) / direction.normalized.x * -hit.normal.x;
        }

        hit.point = hit.point - offset * direction.normalized;

        return hit;
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

            // hit.point = new Vector3(hit.point.x - (ray) * hit.normal.x, hit.point.y, hit.point.z + (ray) * hit.normal.z);
            // print("end -> "+hit.point);
            // print("hit -> "+hit.normal);
            // print("dir -> "+lastDir.normalized);
            // print(hit.normal);
            // print("offset -> "+offset);
            // Debug.DrawLine(transform.position, hit.point, UnityEngine.Color.green);
            // print("start -> "+hit.point);

            hit = RealHit(hit, lastDir);

            points.Add(hit.point);

            if(!Mathf.Approximately(hit.normal.x, 0f) && hit.collider.tag != Tags.bar)
                break;

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

    IEnumerator StopMovement(float time)
    {
        stop = true;
        yield return new WaitForSeconds(time);
        stop = false;
    }

    public BarController GetLastBarTouchedBall()
    {
        return this.lastTouchedBall;
    }

    public void SetLifeTime(float time=5f)
    {
        StartCoroutine(DestroyInSeconds(time));
    }

    IEnumerator DestroyInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        myAnim.SetBool("Disappear", true);
    }

    public void DestroyMe()
    {
        GameplayManager.Instance.RemoveBall(this.gameObject);
        Destroy(this.gameObject, 0.1f);
    }

    public void TurnShadow(bool value)
    {
        sombraAnim.SetBool("Fade", value);
    }


    public void IncrementOrDecrementSpeed(float increment)
    {
        SetSpeed(speed + increment);
    }

    public void SetSpeed(float speedNew)
    {
        speed = Mathf.Clamp(minSpeed, speedNew, maxSpeed);
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
