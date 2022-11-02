using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public FloatVariable PlayerHP, ComputerHP;
    public static GameManager Instance;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(Instance != null || Instance == this)
            Destroy(this.gameObject);

        Instance = this;    
    }

    public void HitLeft()
    {
        PlayerHP.ApplyChange(-1);
    }

    public void HitRight()
    {
        ComputerHP.ApplyChange(-1);
    }

    public void LeftWin()
    {
        print("O lado esquerdo venceu!");
    }

    public void RightWin()
    {
        print("O lado direito venceu!");
    }
}
