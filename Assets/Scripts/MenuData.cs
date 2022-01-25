using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuData: MonoBehaviour
{
    [SerializeField] private bool readyToUse = false;
    [SerializeField] private MenusController menuControlCenter;
    [Header("MenuData")]
    [SerializeField] private TMP_Text volumeTXTValue = null;
    [SerializeField] private Slider VolumeSlider = null;
    [SerializeField] private Toggle fullScreen;

    private void StandBy()
    {
        if (readyToUse)
        {
            if (PlayerPrefs.HasKey("dzwiek"))
            {
                float volume = PlayerPrefs.GetFloat("dzwiek");
                VolumeSlider.value = volume;
                AudioListener.volume = volume;
            }
            if (PlayerPrefs.HasKey("pelenEkran"))
            {
                int screen = PlayerPrefs.GetInt("pelenEkran");
                if(screen == 1)
                {
                    Screen.fullScreen = true;
                    fullScreen.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    fullScreen.isOn = false;
                }
            }
        }
        else
        {
            menuControlCenter.DefaultButton();
        }
        
    }
}
