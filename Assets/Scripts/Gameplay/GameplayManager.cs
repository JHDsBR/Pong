using System;
// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayManager : MonoBehaviour
{

    public GameObject player1, player2, bot;
    public Transform startPointLeftCenter, startPointRightCenter;
    public GameObject Result, ResultWinText, ResultLoseText;
    public GameObject pauseMenu;
    public FloatVariable PlayerScore, ComputerScore, MaxScore;
    public Animator shadowAnim;
    public float ballSpeed;
    // public int powerUpMask;

    [HideInInspector]
    public BarController lastTouchedBall;
    
    [HideInInspector]
    public bool stopGame;
    
    private List<GameObject> allBalls = new List<GameObject>();

    public static GameplayManager Instance;
    public bool shadowOn=false;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null || Instance == this)
            Destroy(this.gameObject);
            
        Instance = this;    
        
        print(Camera.main.aspect);
        print(Camera.main.aspect);
        print(Camera.main.aspect);

        if(Mathf.Approximately((float) System.Math.Round(Camera.main.aspect,1), 1.6f))
            Camera.main.orthographicSize = 6.5f;

        player1.SetActive(false);
        player2.SetActive(false);
        bot.SetActive(false);
        
        Result.SetActive(false);
        ResultWinText.SetActive(false);
        ResultLoseText.SetActive(false);
        PlayerScore.SetValue(MaxScore);
        ComputerScore.SetValue(MaxScore);
        // powerUpMask = LayerMask.GetMask("PowerUp");

        switch(GameManager.Instance.GetCurrentGameMode())
        {
            case 0: // jogando sozinho
                PlayAlone();
                break;
            case 1: // jogando versus
                PlayVersus();
                break;
            case 2: // jogando coop
                PlayCoop();
                break;
        }
        
    }


    public void PlayAlone()
    {
        print("Player alone");

        BarController bar;

        // player/bot sizes
        float minSize   = 0.5f;
        float size      = 1.75f;
        float maxSize   = 6f;

        //player 1
        player1.SetActive(true);
        player1.transform.position = startPointLeftCenter.position;
        bar = player1.GetComponent<BarController>();
        bar.SetLimitTop();
        bar.SetLimitBottom();
        bar.SetAllSizes(minSize, size, maxSize); //min, size, max

        //bot
        bot.SetActive(true);
        bot.transform.position = startPointRightCenter.position;
        bar = bot.GetComponent<BarController>();
        bar.SetLimitTop();
        bar.SetLimitBottom();
        bar.SetAllSizes(minSize, size, maxSize); //min, size, max
    }

    public void PlayVersus()
    {
        print("Player versus");
        ResultWinText.GetComponent<TextMeshProUGUI>().SetText("Jogador 1 Venceu!");
        ResultLoseText.GetComponent<TextMeshProUGUI>().SetText("Jogador 2 Venceu!");

        BarController bar;

        // players sizes
        float playerMinSize   = 0.5f;
        float playerSize      = 1.75f;
        float playerMaxSize   = 6f;

        //player 1
        player1.SetActive(true);
        player1.transform.position = startPointLeftCenter.position;
        bar = player1.GetComponent<BarController>();
        bar.SetLimitTop();
        bar.SetLimitBottom();
        bar.SetAllSizes(playerMinSize, playerSize, playerMaxSize); //min, size, max

        //player 2
        player2.SetActive(true);
        player2.transform.position = startPointRightCenter.position;
        bar = player2.GetComponent<BarController>();
        bar.SetLimitTop();
        bar.SetLimitBottom();
        bar.SetAllSizes(playerMinSize, playerSize, playerMaxSize); //min, size, max

    }

    public void PlayCoop()
    {
        BarController bar;

        // players sizes
        float playerMinSize   = 0.5f;
        float playerSize      = 1.5f;
        float playerMaxSize   = 2.5f;

        // bot sizes
        float botMinSize   = 0.5f;
        float botSize      = 1.5f;
        float botMaxSize   = 2.5f;

        //player 1
        player1.SetActive(true);
        bar = player1.GetComponent<BarController>();
        bar.SetLimitJustToTop();
        bar.SetAllSizes(playerMinSize, playerSize, playerMaxSize); //min, size, max

        //player 2
        player2.SetActive(true);
        bar = player2.GetComponent<BarController>();
        bar.SetLimitJustToBottom();
        bar.SetAllSizes(playerMinSize, playerSize, playerMaxSize); //min, size, max

        //bot
        bot.SetActive(true);
        bar = bot.GetComponent<BarController>();
        bar.SetLimitTop();
        bar.SetLimitBottom();
        bar.SetAllSizes(botMinSize, botSize, botMaxSize); //min, size, max
    }

    public void LeftWin()
    {
        print("O lado esquerdo venceu!");
        stopGame = true;
        ResultWinText.SetActive(true);
        Result.SetActive(true);
    }

    public void RightWin()
    {
        print("O lado direito venceu!");
        stopGame = true;
        ResultLoseText.SetActive(true);
        Result.SetActive(true);
    }

    public void HitLeft()
    {
        PlayerScore.ApplyChange(-1);
        CheckIfWinOrLose();
    }

    public void HitRight()
    {
        ComputerScore.ApplyChange(-1);
        CheckIfWinOrLose();
    }

    private void CheckIfWinOrLose()
    {
        if(PlayerScore.Value <= 0)
            RightWin();
        else if(ComputerScore.Value <= 0)
            LeftWin();
    }

    public void AddNewBall(GameObject ball)
    {
        this.allBalls.Add(ball);
    }

    public void RemoveBall(GameObject ball)
    {
        this.allBalls.Remove(ball);
    }

    public List<GameObject> GetBalls()
    {
        return this.allBalls;
    }

    public void TurnOnShadow(float duration)
    {
        float offSet = 6; // 3 segundos para ativar e mais 3 para desativar
        StopAllCoroutines();
        shadowOn = true;
        shadowAnim.SetBool("Fade", true);
        // foreach (GameObject gb in GetBalls())
        // {
        //     gb.GetComponent<Ball>().TurnShadow(true);
        // }

        StartCoroutine(TurnOffShadow(duration + offSet));
    }
    
    IEnumerator TurnOffShadow(float duration)
    {
        yield return new WaitForSeconds(duration);
        shadowOn = false;
        shadowAnim.SetBool("Fade", false);
        // foreach (GameObject gb in GetBalls())
        // {
        //     gb.GetComponent<Ball>().TurnShadow(false);
        // }
    }

    public void PauseGame()
    {
        if(this.stopGame) return;

        stopGame = true;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        stopGame = false;
        pauseMenu.SetActive(false);
    }
}
