using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerAI : BarController {

    enum STATES { WAITING, REASONING, ACTING }

    Transform trajectoryMarkPrefab;

    List<Vector3> trajectoryPoints = new List<Vector3>();

    STATES state;

    Ball ball;

    float topLimit = 0;
    float bottomLimit = 0;
    float rightLimit = 0;

    float currentPosition;
    float targetPosition;

    float distanceToReact     = 0f;
    float uncertaintyPosition = 0.8f;
    float speed = 5f;

    float randomMove = 0;
    float randomMoveInterval = 1.0f;

    public int side = 1;

    bool showTrajectory = true;
    int collisionMask;
    public LineRenderer lineRenderer;

    void Start() {
        trajectoryMarkPrefab = Resources.Load<Transform>("trajectoryMark");

        state = STATES.WAITING;

        ball = FindObjectOfType<Ball>();

        rightLimit = Camera.main.orthographicSize * Camera.main.aspect + transform.localScale.z / 2;

        randomMove = Time.time + UnityEngine.Random.Range(0, 1.5f);
        collisionMask = LayerMask.GetMask("Solid");
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, collisionMask);
        float distanceFromWall = hit.point.z; // limite baseado no tamanho do campo

        topLimit = Camera.main.orthographicSize; // limite baseado no tamanho da camera
        bottomLimit = -Camera.main.orthographicSize; // limite baseado no tamanho da camera

        topLimit = distanceFromWall; // limite baseado no tamanho da camera
        bottomLimit = -distanceFromWall; // limite baseado no tamanho da camera
        Debug.DrawLine(transform.position, transform.position + transform.forward*distanceFromWall, UnityEngine.Color.green, 50f);
        distanceToReact = transform.position.x - 5;

    }

    public void Update() {

        switch (state) {

            case STATES.WAITING:

                // Se a bolinha está se movendo pra direita (1° condição) e a posição dela ultrapassou o 
                // limite pra IA reagir (2° condição), então, muda o estado pra "REAGINDO"
                if (side == 1) {
                    if (Mathf.Sign(ball.velocity.x) > 0 && ball.transform.position.x >=  distanceToReact) {
                        state = STATES.REASONING;
                    }
                } else {
                    if (Mathf.Sign(ball.velocity.x) < 0 && ball.transform.position.x <= -distanceToReact) {
                        state = STATES.REASONING;
                    }
                }
                // Move a raquete para a posição da bolinha caso o tempo ultrapassou o delay de movimento (1° condição)
                // e se a posição da bola é menor que a distancia para reagir
                if (side == 1) {
                    if (Time.time >= randomMove && ball.transform.position.x <= distanceToReact) {
                        targetPosition = ball.transform.position.z;
                        randomMove = Time.time + UnityEngine.Random.Range(1, 1.5f);
                    }
                } else {
                    if (Time.time >= randomMove && ball.transform.position.x >= -distanceToReact) {
                        targetPosition = ball.transform.position.z;
                        randomMove = Time.time + UnityEngine.Random.Range(1, 1.5f);
                    }
                }
               // Aplica o movimento
               GoToPosition();
               break;

            case STATES.REASONING:

               // calcula a trajetória
               SimulateBallTrajectory();

               currentPosition = transform.position.z;
               
               // Adiciona uma incerteza na posição final. Essa incerteza faz com que a raquete
               // não rebata a bolinha sempre no centro, adicionando mais natualidade
               float targetUp = targetPosition + uncertaintyPosition;
               float targetDown = targetPosition - uncertaintyPosition;
               targetPosition = UnityEngine.Random.Range(targetDown, targetUp);

               // após tudo terminar, muda o estado pra AGINDO
               state = STATES.ACTING;
               break;

            case STATES.ACTING:
               // move-se para a posição final calculada na simulação 
               GoToPosition();

                // caso a bolinha seja rebativa (1° condição) ou ultrapasse o limite da tela, volta
                // pro estado ESPERANDO
                if (side == 1) {
                    if (Mathf.Sign(ball.velocity.x) < 0 || ball.transform.position.x >= 8.5f) {
                        state = STATES.WAITING;
                    }
                } else {
                    if (Mathf.Sign(ball.velocity.x) > 0 || ball.transform.position.x <= -8.5f) {
                        state = STATES.WAITING;
                    }
                }
               break;
        }

        // KeepInsideTheField();

    }
    
    // Faz a raquete se mover suavemente para a posição alvo, respeitando os limites da tela
    void GoToPosition() {

        // currentPosition = Mathf.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        Move(Mathf.Sign(targetPosition-transform.position.z) * speed * Time.deltaTime);

        // transform.position = new Vector3(transform.position.x, transform.position.y, currentPosition);

        // transform.position = new Vector3(transform.position.x,
                                // transform.position.y,
                                // Mathf.Clamp(transform.position.z,
                                // bottomLimit + transform.localScale.z / 2,
                                // topLimit - transform.localScale.z / 2));

        // currentPosition = Mathf.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        // transform.position = new Vector3(transform.position.x, transform.position.y, currentPosition);

        // transform.position = new Vector3(transform.position.x,
        //                         transform.position.y,
        //                         Mathf.Clamp(transform.position.z,
        //                         bottomLimit + transform.localScale.z / 2,
        //                         topLimit - transform.localScale.z / 2));

    }

    void SimulateBallTrajectory() {

        
        List<Vector3> points = new List<Vector3>(){ball.transform.position};

        RaycastHit hit;
        Vector3 lastPos = ball.transform.position;
        Vector3 lastDir = ball.velocity;

        for (var i = 0; i < 5; i++)
        {
            Physics.Raycast(lastPos, lastDir, out hit, Mathf.Infinity, collisionMask);
            
            if(hit.collider == null)
                break;
            // print(lastDir.normalized * ball.ray);
            hit.point = hit.point - lastDir.normalized * (ball.ray + 0.075f);

            points.Add(hit.point);

            if(hit.point.x > transform.position.x)
                break;

            lastDir = Vector3.Reflect(lastDir, hit.normal);
            lastPos = hit.point;
        }

        // lineRenderer.positionCount = points.Count;
        int idx=0;

        foreach (Vector3 point in points)
        {
            // lineRenderer.SetPosition(idx, point);
            targetPosition = point.z;
            idx++;
        }


        return;

        int iterations = 3;
        float step     = 10f;

        Vector3 position = ball.transform.position;
        Vector3 velocity = ball.velocity;

        while (iterations > 0) {

            position += velocity * step;

            if(position.z >= topLimit || position.z <= bottomLimit) {
                velocity.z *= -1;
            }

            if (side == 1) {
                if (position.x >= 8.2f) {
                    targetPosition = position.z;
                    break;
                }
            } else {
                if (position.x <= -8.2f) {
                    targetPosition = position.z;
                    break;
                }
            }

            --iterations;

            if (showTrajectory)
                trajectoryPoints.Add(position);
            
        }
        // StartCoroutine(DrawTrajectory());

        for (int i = 0; i < trajectoryPoints.Count; ++i) {
            Instantiate(trajectoryMarkPrefab, trajectoryPoints[i], trajectoryMarkPrefab.rotation);
        }
        trajectoryPoints.Clear();
    }

    // // Desenha a trajetória simulada do movimento da bola
    // IEnumerator DrawTrajectory() {

    //     for (int i = 0; i < trajectoryPoints.Count; ++i) {
    //         Instantiate(trajectoryMarkPrefab, trajectoryPoints[i], Quaternion.identity);
    //         yield return null;
    //     }
    //     trajectoryPoints.Clear();

    // }

    // public void Reset() {
    //     transform.position = new Vector3(transform.position.x, 0);
    // }

}
