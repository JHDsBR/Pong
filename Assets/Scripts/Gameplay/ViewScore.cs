using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewScore : MonoBehaviour
{
    [Header("Player")]
    public TextMesh TextPlayerScore;
    public FloatVariable PlayerScore;
    
    [Header("Computer")]
    public TextMesh TextComputerScore;
    public FloatVariable ComputerScore;
    
    [Header("Everyone")]
    public FloatVariable MaxScore;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        MaxScore.SetValue(Mathf.Abs(MaxScore.Value));
        if(Mathf.Approximately(MaxScore.Value, 0f))
        {
            Debug.LogWarning("O valor máximo para a pontuação deve ser diferente de zero");
            MaxScore.SetValue(1);
        }
        PlayerScore.SetValue(MaxScore);
        ComputerScore.SetValue(MaxScore);
        
        StartCoroutine("ShowScore");
    }

    
    IEnumerator ShowScore()
    {
        while(true)
        {
            TextPlayerScore.text = PlayerScore.Value+"";
            TextComputerScore.text = ComputerScore.Value+"";
            yield return null;
        }
    }

}
