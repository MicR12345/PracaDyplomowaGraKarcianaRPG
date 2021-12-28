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

    GameObject playerObject;
    [HideInInspector]
    public Player player;
    
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
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("cardTest2")));
        player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("cardTest3")));
        player.CreateCardStack();  
        //player.AddCardToPlayerHand();
        //player.AddCardToPlayerHand();
        //player.AddDirectCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        //player.AddDirectCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("cardTest2")));
        //player.AddDirectCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("cardTest3")));
        player.RefreshHand();
    }
    void CreatePlayerObject()
    {
        playerObject = new GameObject();
        playerObject.transform.parent = this.gameObject.transform;
        playerObject.transform.localPosition = Vector3.zero;
        player = playerObject.AddComponent<Player>();
        player.PlayerObjectSetup(this, null, null);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
