using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Event
{
    public void Active(Ball ball);
    public void DestroyMe();
    public void SetLifeTime(float value);
}

public class EventClass : MonoBehaviour
{
    public GameObject lifeTimeBar;
    public Animator myAnim;
    public AudioSource myAudio;

    public void DestroyMe()
    {
        myAnim.SetBool("PickUp", true);
        Destroy(transform.parent.gameObject, 1f);
    }

    public void SetLifeTime(float time)
    {
        StartCoroutine(UpdateLifeTimeBar(Mathf.Abs(time)));
    }

    IEnumerator UpdateLifeTimeBar(float lifeTime)
    {
        Vector3 scale = lifeTimeBar.transform.localScale;
        float startScaleX = scale.x, time=0;

        while(time < lifeTime)
        {
            time += Time.deltaTime;
            lifeTimeBar.transform.localScale = new Vector3(startScaleX-(startScaleX*time/lifeTime), scale.y, scale.z); 
            yield return null;
        }

        lifeTimeBar.transform.localScale = new Vector3(0, scale.y, scale.z); 
    }
}


