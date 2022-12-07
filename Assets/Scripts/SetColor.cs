using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour
{
    public SpriteRenderer sprite;
    
    [Range(0f, 1f)]
    public float opacity;

    // Start is called before the first frame update
    void Start()
    {
        Color color     = GameManager.Instance.GetCurrentColor();
        color[3]        = opacity;
        sprite.color    = color;
    }
}
