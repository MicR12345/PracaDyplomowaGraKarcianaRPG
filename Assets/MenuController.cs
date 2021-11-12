using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Voleme Settings")]
    [SerializeField] private TMP_Text VolumeValue = null;
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private GameObject comfirmationPrompt = null;

    public object AduioListener { get; private set; }

    public void ExitButton()
    {
        Application.Quit();
    }
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        VolumeValue.text = volume.ToString("0.0");
    }
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmVolume());
    }
    public IEnumerator ConfirmVolume()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(5);
        comfirmationPrompt.SetActive(false);
    }
}
