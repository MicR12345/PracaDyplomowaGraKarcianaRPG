using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EndGameManager : MonoBehaviour
{
    public GameObject skipObject;
    public Image fadeInImage;
    // Start is called before the first frame update
    float skipTimer = 1f;

    float fadeInTime = 5f;
    void Start()
    {
        StartCoroutine(FadeInImage());
    }
    IEnumerator FadeInImage()
    {
        for (float i = 0; i < fadeInTime; i = i + Time.deltaTime)
        {

            fadeInImage.color = new Color(1, 1, 1, i / fadeInTime);
            yield return null;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (skipTimer>0)
        {
            skipTimer = skipTimer - Time.deltaTime;
        }
        else
        {
            skipObject.SetActive(true);
        }
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
