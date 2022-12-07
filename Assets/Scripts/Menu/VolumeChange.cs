using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChange : MonoBehaviour
{
    public UnityEngine.UI.Slider mySlider;
    // Start is called before the first frame update
    void Start()
    {
        mySlider.value = GameManager.Instance.GetVolume();
    }
    
    public void ValueChangeCheck(float value)
    {
        GameManager.Instance.SetVolume(value);
    }
}
