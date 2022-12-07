using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractEvent : EventClass, Event 
{
    public void Active(Ball ball)
    {
        BarController bar = ball.GetLastBarTouchedBall();
        
        if(bar == null)
            return;

        myAudio.Play(0);
        
        bar.Contract();
        DestroyMe();
    }

    // public void DestroyMe()
    // {
    //     DestroyMe_();
    // }

    // public void SetLifeTime(float time)
    // {
    //     SetLifeTime_(time);
    // }
}
