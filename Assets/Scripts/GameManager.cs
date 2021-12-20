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
    Player player;
    
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
        player.AddCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        player.AddCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("cardTest2")));
        player.AddCardToPlayerHand(new DeckCard(cardLibrary.FindCardByName("cardTest3")));
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
