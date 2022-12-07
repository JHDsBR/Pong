using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColor : MonoBehaviour
{
    public TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        trail.material.color = GameManager.Instance.GetCurrentColor();
    }
}
