using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject buttons, multiplayer, settings;
    public GameObject firstElementInMenu, firstElementInMultiplayer, firstElementInSettings;

    public void OpenMenu()
    {
        buttons.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstElementInMenu);    
        multiplayer.SetActive(false);
        settings.SetActive(false);
    }

    public void OpenMultiplayer()
    {
        multiplayer.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstElementInMultiplayer);    
        buttons.SetActive(false);
    }

    public void OpenSettings()
    {
        settings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstElementInSettings);    
        buttons.SetActive(false);
    }

    public void PlayAlone()
    {
        GameManager.Instance.SetCurrentGameMode(0);
    }

    public void PlayVersus()
    {
        GameManager.Instance.SetCurrentGameMode(1);
    }

    public void PlayCoop()
    {
        GameManager.Instance.SetCurrentGameMode(2);
    }
}
