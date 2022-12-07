using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Player{Jogador1=1, Jogador2=2}

public class BarController : MonoBehaviour
{
    public Player playerNum;
    public float movementSpeed, size, maxSize, minSize;
    public float movementSpeedMin, movementSpeedMax;

    float distanceFromWall;

    float limitTop, limitBottom;

    private Controllers control;

    private float posiUp, posiDown;

    bool receivingInputFromJoystick;
    // Start is called before the first frame update
    void Start()
    {
        control = new Controllers();
        control.Enable();
        // control.Gameplay.Up.performed += ctx => posiUp = ctx.ReadValue<float>();
        // control.Gameplay.Up.canceled += ctx => posiUp = 0;
        // control.Gameplay.Down.performed += ctx => posiDown = -ctx.ReadValue<float>();
        // control.Gameplay.Down.canceled += ctx => posiDown = 0;
        // print("POSITION -> "+transform.position);
        // distanceFromWall = Camera.main.orthographicSize - transform.localScale.z/2; // limite baseado no tamanho da tela
        SetStartLocalScale();
        // StartCoroutine(ChangeSize(size+2,4));
        // Expand();
    }

    // Update is called once per frame
    void Update()
    {
        // print(Input.GetAxis("Vertical" + (int) playerNum));
        if(GameplayManager.Instance.stopGame)
            return;

        if(Mathf.Approximately(posiUp + posiDown, 0f))
            receivingInputFromJoystick = false;

        // transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y,this.size);
        // Movement();
        
        Movement();
        Move((posiUp + posiDown) * movementSpeed * Time.deltaTime);
        // Move(Input.GetAxis("Vertical" + (int) playerNum) * movementSpeed * Time.deltaTime);
        KeepInsideTheField();
    }

    // void Movement()
    // {
    //     dir = Input.GetAxis("Vertical");
    //     transform.position += movementSpeed * Time.deltaTime * dir * transform.forward;
    //     transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, limitBottom+size/2, limitTop-size/2));
    // }


    public void Movement()
    {
        if (receivingInputFromJoystick) return;

        if((int) playerNum == 1)
        {
            if(Input.GetKey("w"))
            {
                posiUp = 1;
            }
            else
            {
                posiUp = 0;
            }
            if(Input.GetKey("s"))
            {
                posiDown = -1;
            }
            else
            {
                posiDown = 0;
            }
        }
        else if((int) playerNum == 2)
        {
            if(Input.GetKey("up"))
            {
                posiUp = 1;
            }
            else
            {
                posiUp = 0;
            }
            if(Input.GetKey("down"))
            {
                posiDown = -1;
            }
            else
            {
                posiDown = 0;
            }
        }
    }

    public void MoveUp(InputAction.CallbackContext context)
    {
        receivingInputFromJoystick = true;
        print("UP");
        posiUp      = context.ReadValue<float>();
    } 

    public void MoveDown(InputAction.CallbackContext context)
    {
        receivingInputFromJoystick = true;
        print("DOWN");
        posiDown    = -context.ReadValue<float>();
    } 

    public float GetDirectionInput()
    {
        print("INPUT: "+ posiUp +" : "+ posiDown);
        // return Input.GetAxis("Vertical" + (int) playerNum);
        return (posiUp + posiDown);
    }

    public void SetSize(float newSize)
    {
        this.size = Mathf.Clamp(this.minSize, newSize, this.maxSize);
        // StartCoroutine(ChangeSize(newSize));
    }

    public void Expand()
    {
        float newSize = this.size + 0.2f;
        newSize = Mathf.Clamp(this.minSize, newSize, this.maxSize);
        StartCoroutine(ChangeSize(newSize,2));
        // StartCoroutine(ChangeSize(this.size+=1, 3));
    }

    public void Contract()
    {
        float newSize = this.size - 0.2f;
        newSize = Mathf.Clamp(this.minSize, newSize, this.maxSize);
        StartCoroutine(ChangeSize(newSize,2));
        // StartCoroutine(ChangeSize(this.size+=1, 3));
    }

    public void SetMaxSize(float newMaxSize)
    {
        this.maxSize = newMaxSize;
    }

    public void SetMinSize(float newMinSize)
    {
        this.minSize = newMinSize;
    }

    public void SetAllSizes(float min, float value, float max)
    {
        this.SetMinSize(min);
        this.SetSize(value);
        this.SetMaxSize(max);
    }

    IEnumerator ChangeSize(float newSize, float duration=0.5f)
    {
        if(!Mathf.Approximately(newSize, 0f) || newSize <= 0 || Mathf.Approximately(newSize,this.size))
            yield return null;

        float oldSize =this.size;
        float step = (newSize - oldSize) / duration;
        float total = 0, currentStep;
        
        while(Mathf.Abs(total) < Mathf.Abs(newSize - oldSize))
        {
            currentStep = step*Time.deltaTime;
            total += currentStep;
            transform.localScale += Vector3.forward*currentStep;
            this.size = transform.localScale.z;
            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newSize);
    }

    
    // mantém a barra dentro do campo
    protected void KeepInsideTheField()
    {
        Vector3 myPos = transform.position;
        float myZ = myPos.z;
        
        if(myZ > limitTop -this.size/2) // passou do limite superior
            myPos.z = limitTop -this.size/2;
        else if(myZ < limitBottom +this.size/2) // passou do limite inferior
            myPos.z = limitBottom +this.size/2;

        transform.position = myPos; 
    }

    protected void Move(float step)
    {
        transform.position += transform.forward * step;
    }

    public void SetLimitTop()
    {
        RaycastHit hit;
        Physics.Raycast(transform.forward*transform.position.z, Vector3.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Solid"));
        // distanceFromWall = hit.point.z - transform.localScale.z/2; // limite baseado no tamanho do campo
        limitTop = hit.point.z;
        // print("Set limit top -> "+limitTop);
    }

    public void SetLimitBottom()
    {
        RaycastHit hit;
        Physics.Raycast(transform.forward*transform.position.z, -Vector3.forward, out hit, Mathf.Infinity, LayerMask.GetMask("Solid"));
        limitBottom = hit.point.z;
        print("Set limit bottom -> "+transform.forward*transform.position.z);
    }

    public void SetLimitJustToTop()
    {
        print("Set limit just to top");
        SetLimitTop();
        this.limitBottom = 0;
    }

    public void SetLimitJustToBottom()
    {
        print("Set limit just to bottom");
        SetLimitBottom();
        this.limitTop = 0;
    }

    public void SetStartLocalScale()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, this.size);
    }

    public void IncrementOrDecrementSpeed(float increment)
    {
        movementSpeed = Mathf.Clamp(movementSpeedMin, movementSpeed + increment, movementSpeedMax);
    }
}
