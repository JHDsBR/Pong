using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEvents : MonoBehaviour
{
    public float lifeTime;
    public Collider col;
    public GameObject[] events;
    private GameObject lastEvent;

    public int eventsSimultaneously;
    // Start is called before the first frame update
    void Start()
    {
        // lastEvent=null;
        for (var c = 0; c < eventsSimultaneously; c++)
            StartCoroutine(Spawn(c*3));
        // lastEvent = Instantiate(events[3]);
        // lastEvent.transform.position = RandomPointInBounds();
    }

    public Vector3 RandomPointInBounds() {
        Bounds bounds = col.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    IEnumerator Spawn(float wait)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(wait, 2+wait));
        GameObject thisLastEvent=null;

        while(true)
        {
            // yield return new WaitUntil(() => lastEvent == null || new WaitForSeconds(lifeTime));
            // if(lastEvent != null)
            // {
            //     last
            // }

            float time=0f;
            
            while(time < lifeTime)
            {
                time += Time.deltaTime;
                
                if(thisLastEvent == null)
                    break;
                
                yield return null;
            }

            if(time >= lifeTime && thisLastEvent != null)
            {
                Event ev = thisLastEvent.transform.GetChild(0).GetComponent<Event>();
                ev.DestroyMe();
                yield return new WaitForSeconds(UnityEngine.Random.Range(4f,7f));
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(3,8));
            }

            thisLastEvent = Instantiate(events[UnityEngine.Random.Range(0, events.Length)]);
            thisLastEvent.transform.GetChild(0).GetComponent<Event>().SetLifeTime(lifeTime);
            thisLastEvent.transform.position = RandomPointInBounds();

            yield return null;
        }
    }
}
