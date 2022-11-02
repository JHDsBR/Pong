using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public float movementSpeed, size;

    float distanceFromWall;
    float dir;

    float limitTop, limitBottom;
    // Start is called before the first frame update
    void Start()
    {
        // distanceFromWall = Camera.main.orthographicSize - transform.localScale.z/2; // limite baseado no tamanho da tela
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size);
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Solid"));
        // distanceFromWall = hit.point.z - transform.localScale.z/2; // limite baseado no tamanho do campo
        limitTop = hit.point.z;
        Physics.Raycast(transform.position, -Vector3.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Solid"));
        limitBottom = hit.point.z;
        StartCoroutine(ChangeSize(5,10));
    }

    // Update is called once per frame
    void Update()
    {
        // transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size);
        Movement();
        // KeepInsideTheField();
    }

    void Movement()
    {
        dir = Input.GetAxis("Vertical");
        transform.position += movementSpeed * Time.deltaTime * dir * transform.forward;
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, limitBottom+size/2, limitTop-size/2));
    }

    public float GetDirectionInput()
    {
        return dir;
    }

    IEnumerator ChangeSize(float newSize, float duration)
    {
        if(!Mathf.Approximately(newSize, 0f) || newSize <= 0 || Mathf.Approximately(newSize, size))
            yield return null;

        float oldSize = size;
        float step = (newSize - oldSize) / duration;
        float total = 0, currentStep;
        
        while(total < (newSize - oldSize))
        {
            currentStep = step*Time.deltaTime;
            total += currentStep;
            transform.localScale += Vector3.forward*currentStep;
            size = transform.localScale.z;
            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newSize);
    }

    
    // mantém a barra dentro do campo
    void KeepInsideTheField()
    {
        Vector3 myPos = transform.position;
        float myZ = myPos.z;
        
        if(myZ > limitTop - size/2) // passou do limite superior
            myPos.z = limitTop - size/2;
        else if(myZ < limitBottom + size/2) // passou do limite inferior
            myPos.z = limitBottom + size/2;

        transform.position = myPos; 
    }
}
