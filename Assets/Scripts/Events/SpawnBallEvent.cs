using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallEvent : EventClass, Event
{
    public float lifeTime;
    public GameObject ballPrefab;

    public void Active(Ball ball)
    {
        GameObject newBall = Instantiate(ballPrefab);
        newBall.GetComponent<Ball>().SetLifeTime(lifeTime);
        newBall.transform.position = Vector3.zero;

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
