using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
    public float offSet=0;
    Component[] audios;
    // Start is called before the first frame update
    void Start()
    {
        audios = this.gameObject.GetComponents(typeof(AudioSource));
        foreach (AudioSource audio in audios)
        {
            audio.volume = GameManager.Instance.GetVolume()+offSet;
        }
    }

}
