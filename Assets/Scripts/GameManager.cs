using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Background;
    public GameObject GameLight;
    public Camera GameCamera;
    public GameObject CardLibrary;
    CardLibrary cardLibrary;
    void Start()
    {
        cardLibrary = CardLibrary.GetComponent<CardLibrary>();
        cardLibrary.LoadAllCards();
        Debug.Log(cardLibrary.cards.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
