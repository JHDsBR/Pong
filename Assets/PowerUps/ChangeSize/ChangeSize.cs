using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSize : MonoBehaviour, PowerUp
{

    public void StartEffect()
    {
        BarController bar = GameplayManager.Instance.lastTouchedBall;
        // bar.SetSize(bar.size+1);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag != Tags.ball)
            return;
            
        StartEffect();
    }
}
