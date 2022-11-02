using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHP : MonoBehaviour
{
    [Header("Player")]
    public Image ImagePlayerHP;
    public FloatVariable PlayerHP;
    public FloatVariable MaxPlayerHP;
    
    [Header("Computer")]
    public Image ImageComputerHP;
    public FloatVariable ComputerHP;
    public FloatVariable MaxComputerHP;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MaxPlayerHP.SetValue(Mathf.Abs(MaxPlayerHP.Value));
        if(Mathf.Approximately(MaxPlayerHP.Value, 0f))
        {
            Debug.LogWarning("O valor máximo para vida do player deve ser diferente de zero");
            MaxPlayerHP.SetValue(1);
        }
        PlayerHP.SetValue(MaxPlayerHP);

        MaxComputerHP.SetValue(Mathf.Abs(MaxComputerHP.Value));
        if(Mathf.Approximately(MaxComputerHP.Value, 0f))
        {
            Debug.LogWarning("O valor máximo para vida do computador deve ser diferente de zero");
            MaxComputerHP.SetValue(1);
        }
        ComputerHP.SetValue(MaxComputerHP);
        
        StartCoroutine("ShowHP");
    }

    
    IEnumerator ShowHP()
    {
        while(true)
        {
            ImagePlayerHP.fillAmount = PlayerHP.Value/MaxPlayerHP.Value;
            ImageComputerHP.fillAmount = ComputerHP.Value/MaxComputerHP.Value;
            yield return null;
        }
    }

}
