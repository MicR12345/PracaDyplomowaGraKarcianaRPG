using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TMP_Text sensitivityValue = null;
    [SerializeField] private Slider sensitivitySlider = null;

    [SerializeField] private float defaultVolumeSettings = 0.5f;
    [SerializeField] private float defaultSensitivitySettings = 30f;

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
        if (PlayerPrefs.HasKey("Zapis"))
        {
            _currentGameSave = PlayerPrefs.GetString("Zapis");
            SceneManager.LoadScene(_currentGameSave);
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
    public void SetSensiviti(float Sensivity)
    {
        mainSensitivity = Sensivity;
        sensitivityValue.text = Sensivity.ToString("0");
    }
    public void DefaultButton()
    {
        AudioListener.volume = defaultVolumeSettings;
        volumeSlider.value = defaultVolumeSettings;
        volumeValue.text = defaultVolumeSettings.ToString("0.5");
        SetVolume(defaultVolumeSettings);
        mainSensitivity = defaultSensitivitySettings;
        sensitivitySlider.value = defaultSensitivitySettings;
        sensitivityValue.text = defaultSensitivitySettings.ToString("0");
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("dzwiek", AudioListener.volume);
        PlayerPrefs.SetFloat("czulosc", mainSensitivity);
    }
}

