using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBarEvent : EventClass, Event 
{
    public float increment;

    public void Active(Ball ball)
    {
        BarController bar = ball.GetLastBarTouchedBall();
        
        if(bar == null)
            return;

        myAudio.Play(0);
        
        bar.IncrementOrDecrementSpeed(increment);
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
