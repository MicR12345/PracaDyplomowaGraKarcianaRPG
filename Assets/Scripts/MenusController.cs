using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenusController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeValue = null;
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private GameObject comfirmPrompt = null;

    [Header("SavedGames")]
    public string _newGameSave;
    private string _currentGameSave;
    [SerializeField] private  GameObject NoSavedGameDialog = null;
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

    public void Save()
    {
        PlayerPrefs.SetFloat("dzwiek", AudioListener.volume);
        StartCoroutine(Confirm());
    }
    public IEnumerator Confirm()
    {
        comfirmPrompt.SetActive(true);
        yield return new WaitForSeconds(3);
        comfirmPrompt.SetActive(false);
    }
}
