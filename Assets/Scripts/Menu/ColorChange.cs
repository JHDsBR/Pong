using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    // Drag & drop slider
    public UnityEngine.UI.Slider mySlider;

    // Drag & drop handle
    public UnityEngine.UI.Image handle;

    public void Start()
    {
        float h,s,v;
        Color.RGBToHSV(GameManager.Instance.GetCurrentColor(), out h, out s, out v);
        mySlider.value = h;
        ValueChangeCheck(mySlider.value);
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck(float value)
    {
        Color newColor = Color.HSVToRGB(value, 1, 1);
        handle.color = newColor;
        GameManager.Instance.SetCurrentColor(newColor);
    }

}
