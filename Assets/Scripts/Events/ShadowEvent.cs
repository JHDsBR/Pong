using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEvent : EventClass, Event 
{
    public float effectDuration;

    public void Active(Ball ball)
    {
        GameplayManager.Instance.TurnOnShadow(effectDuration);

        myAudio.Play(0);
        
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
