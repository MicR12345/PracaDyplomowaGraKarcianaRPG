using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSingleton : MonoBehaviour
{
    public static AudioSingleton singleton;
    public static AudioSource audioSource;
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            audioSource = this.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    static public void PlayMusic(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
