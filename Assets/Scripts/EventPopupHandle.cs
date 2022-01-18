using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventPopupHandle : MonoBehaviour
{
    public GameObject eventPopupCanvas;

    public Image eventBackgroundImage;

    public TextMeshProUGUI descriptionText;

    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;

    public Text button1Text;
    public Text button2Text;
    public Text button3Text;
    public Text button4Text;

    GameManager gameManager;
    void EventChoice1()
    {
        Debug.Log("Test");
        gameManager.HandleEvent(0);
        eventPopupCanvas.SetActive(false);
    }
    void EventChoice2()
    {
        gameManager.HandleEvent(1);
        eventPopupCanvas.SetActive(false);
    }
    void EventChoice3()
    {
        gameManager.HandleEvent(2);
        eventPopupCanvas.SetActive(false);
    }
    void EventChoice4()
    {
        gameManager.HandleEvent(3);
        eventPopupCanvas.SetActive(false);
    }
    private void Awake()
    {
        GameObject world = GameObject.Find("World");
        gameManager = world.GetComponent<GameManager>();
    }
}
