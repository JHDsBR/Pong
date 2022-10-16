using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side{left=1, right=2};

public class LimiteManager : MonoBehaviour
{
    public Side mySide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag != Tags.ball) return;

        if((int) mySide == 1)
            GameManager.RightWin();
        else
            GameManager.LeftWin();
    }
}
