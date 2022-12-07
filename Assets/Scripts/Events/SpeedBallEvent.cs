using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBallEvent : EventClass, Event 
{
    public float increment;

    public void Active(Ball ball)
    {
        myAudio.Play(0);

        GameplayManager.Instance.ballSpeed += increment;

        foreach (GameObject gb in GameplayManager.Instance.GetBalls())
        {
            gb.GetComponent<Ball>().IncrementOrDecrementSpeed(increment);
        }

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
