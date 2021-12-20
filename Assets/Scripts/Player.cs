using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int charcterId;
    public int petId;
    public int healthMax;
    public int health;
    public int actionPointsMax;
    public int actionPoints;
    public int initiative;
    public List<DeckCard> deck;
    public int deckSize;
    public List<DeckCard> cardStack;
    public int stackKnowledge;
    public List<DeckCard> hand;
    public int handSize;
    public List<Card> abilityCards;
    public List<Effect> activeEffects;

    Sprite playerSprite;

    SpriteRenderer spriteRenderer;
    Material spriteMaterial;

    GameManager gm;

    public GameObject Hand;
    List<GameObject> cardsInHand;
    public void PlayerObjectSetup(GameManager _gm, Sprite _playerSprite,Material _spriteMaterial)
    {
        this.name = "Player";
        playerSprite = _playerSprite;
        spriteMaterial = _spriteMaterial;

        gm = _gm;

        spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSprite;

        Hand = new GameObject();
        Hand.name = "Hand";
        Hand.transform.parent = this.gameObject.transform;
        Hand.transform.localPosition = new Vector3(0f, -30f, -1f);

        cardsInHand = new List<GameObject>();

        hand = new List<DeckCard>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCardToPlayerHand(DeckCard card)
    {
        hand.Add(card);
        SetupCardLocation();
        card.card.cardObject.transform.localScale = new Vector3(0.6f, 0.6f);
    }
    void SetupCardLocation()
    {
        Vector3 BasePosition = Hand.transform.position;
        float startPos = -cardsInHand.Count * 11f;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            cardsInHand[i].transform.position = BasePosition + new Vector3(startPos + startPos * i, 0f, 0f);
        }
    }
    public void RefreshHand()
    {
        foreach (GameObject inHand in cardsInHand)
        {
            GameObject.Destroy(inHand);
        }
            foreach (DeckCard item in hand)
        {
            if (item.card.cardObject == null)
            {
                item.card.CreateCardInstance();
            }
        }
    }
}
