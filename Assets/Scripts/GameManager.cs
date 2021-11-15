using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject Background;
    public GameObject GameLight;
    public Camera GameCamera;
    //shared card library
    public GameObject CardLibrary;
    [HideInInspector]
    public CardLibrary cardLibrary;

    GameObject Player;
    PlayerObject player;
    

   
    void Start()
    {
        cardLibrary = CardLibrary.GetComponent<CardLibrary>();
        cardLibrary.LoadAllCards();
        cardLibrary.DebugPrintAllCardsNames();
        OnGameStart();
    }
    void OnGameStart()
    {
        CreatePlayerObject();
        player.AddCardToPlayerHand(cardLibrary.FindCardByName("0_cardTest").card);
        player.AddCardToPlayerHand(cardLibrary.FindCardByName("0_cardTest").card);
        player.RefreshHand();
    }
    void CreatePlayerObject()
    {
        Player = new GameObject();
        Player.transform.parent = this.gameObject.transform;
        Player playerinfo = new Player();
        Player.transform.localPosition = Vector3.zero;
        player = Player.AddComponent<PlayerObject>();
        player.PlayerObjectSetup(this, playerinfo, null, null);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
