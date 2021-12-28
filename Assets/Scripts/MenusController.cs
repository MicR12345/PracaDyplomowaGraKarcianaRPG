using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenusController : MonoBehaviour
{
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
}
