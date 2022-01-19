using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenusController : MonoBehaviour
{
    [Header("Game Settings")]
    public float mainSensitivity = 30;
    [SerializeField] private TMP_Text volumeValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolumeSettings = 0.5f;
    private bool _fullScreen;

    [Header("SavedGames")]
    public string _newGameSave;
    private string _currentGameSave;
    [SerializeField] private GameObject NoSavedGameDialog = null;
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameSave);
    }
    public void LoadGameDialogGrajDalej()
    {
        if (File.Exists(Application.dataPath + "/save.savegame"))
        {
            SceneManager.LoadScene("SaveLoadScreen");
        }
        else
        {
            NoSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float Volume)
    {
        AudioListener.volume = Volume;
        volumeValue.text = Volume.ToString("0.0");
    }

    public void DefaultButton()
    {
        AudioListener.volume = defaultVolumeSettings;
        volumeSlider.value = defaultVolumeSettings;
        volumeValue.text = defaultVolumeSettings.ToString("0.5");
        SetVolume(defaultVolumeSettings);

    }
    public void FullScreenChange(bool fullScreen)
    {
        _fullScreen = fullScreen;
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("dzwiek", AudioListener.volume);
        PlayerPrefs.SetInt("pelenEkran", (_fullScreen ? 1 : 0));
        Screen.fullScreen = _fullScreen;
    }
}

