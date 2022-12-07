using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum GameModes{SINGLE=0, VERSUS=1, COOP=2}

public class GameManager : MonoBehaviour
{
    public Material campoColor;
    private int currentGameMode; // SINGLE=0, VERSUS=1, COOP=2
    private Color currentColor;
    private AudioSource[] sources;
    private float volume;
    public static GameManager Instance;



    void Start()
    {
        UpdateOnLoad();
    }

    void OnLevelWasLoaded()
    {
        UpdateOnLoad();
    }

    void UpdateOnLoad()
    {
        this.sources = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
        SetSourceVolume();
    }

    void Awake()
    {
        if(Instance != null || Instance == this)
            Destroy(this.gameObject);
            
        SetCurrentColor(new Color(PlayerPrefs.GetFloat("red",0.08715761f),
                                  PlayerPrefs.GetFloat("green",1),
                                  PlayerPrefs.GetFloat("blue",0), 1));
        
        SetVolume(PlayerPrefs.GetFloat("volume",0.7f));

        Instance = this;    
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetCurrentGameMode(int newGameMode)
    {
        if(newGameMode < 0 || newGameMode > 2)
            return;

        this.currentGameMode = newGameMode;
    }
    
    public int GetCurrentGameMode()
    {
        return this.currentGameMode;
    }

    public void SetCurrentColor(Color color)
    {
        PlayerPrefs.SetFloat("red",color[0]);
        PlayerPrefs.SetFloat("green",color[1]);
        PlayerPrefs.SetFloat("blue",color[2]);

        this.currentColor = color;
        
        campoColor.SetColor("_Color", color);
    }

    public Color GetCurrentColor()
    {
        return this.currentColor;
    }

    public void SetSourceVolume()
    {
        if(this.sources == null)
            return;

        foreach (AudioSource audio in this.sources)
            audio.volume = GetVolume();
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        this.volume = volume;
        SetSourceVolume();
    }

    public float GetVolume()
    {
        return this.volume;
    }
}
