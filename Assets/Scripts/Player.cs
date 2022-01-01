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
    public int deckSize = 30;
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

        deck = new List<DeckCard>();
        hand = new List<DeckCard>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddToPlayerDeck(DeckCard card)
    {
        if (deck.Count>deckSize)
        {
            Debug.LogError("More cards than possible in deck");
        }
        card.card.CreateCardInstance();
        card.card.cardObject.SetActive(false);
        deck.Add(card);

    }
    public void CreateCardStack(bool random = true)
    {
        cardStack = new List<DeckCard>(deck);
        Debug.Log("Created card stack with " + cardStack.Count + " cards.");
        if (random)
        {
            for (int i = 0; i < cardStack.Count -1; i++)
            {

                int swapPosition = UnityEngine.Random.Range(0, cardStack.Count - 1);
                if (swapPosition != i)
                {
                    DeckCard card1 = cardStack[i];
                    DeckCard card2 = cardStack[swapPosition];
                    cardStack[i] = card2;
                    cardStack[swapPosition] = card1;
                }
            }
        }
    }
    public void AddCardToPlayerHand()
    {
        if (cardStack.Count>0)
        {
            if (
                !cardStack[0].discarded &&
                !cardStack[0].destroyed &&
                cardStack[0].exhausted==0
                )
            {
                
                hand.Add(cardStack[0]);
                cardStack[0].card.cardObject.SetActive(true);
                cardStack.RemoveAt(0);
                SetupCardLocation();
            }
            else
            {
                cardStack.RemoveAt(0);
                AddCardToPlayerHand();
                return;
            }
        }
        else
        {
            RemoveAllFromHand();
            Shuffle();
        }
    }
    public void Shuffle(bool random = true)
    {
        cardStack.Clear();
        foreach (DeckCard item in deck)
        {
            item.inHand = false;
        }
        foreach (DeckCard item in hand)
        {
            item.inHand = true;
        }
        foreach (DeckCard item in deck)
        {
            if (!item.discarded && !item.destroyed && item.exhausted==0 && !item.inHand)
            {
                cardStack.Add(item);
            }
        }
        if (random)
        {
            for (int i = 0; i < cardStack.Count; i++)
            {

                int swapPosition = UnityEngine.Random.Range(0, cardStack.Count - 1);
                if (swapPosition != i)
                {
                    DeckCard card1 = cardStack[i];
                    DeckCard card2 = cardStack[swapPosition];
                    cardStack[i] = card2;
                    cardStack[swapPosition] = card1;
                }
            }
        }
    }
    public void AddDirectCardToPlayerHand(DeckCard card)
    {
        hand.Add(card);
        SetupCardLocation();
    }
    public void RemoveAllFromHand()
    {
        foreach (DeckCard item in deck)
        {
            item.inHand = false;
        }
        foreach (DeckCard item in hand)
        {
            item.inHand = true;
        }
        hand.Clear();
        foreach (DeckCard item in deck)
        {
            if (item.inHand && item.staysInHand)
            {
                hand.Add(item);
            }
            else if(item.inHand && !item.staysInHand)
            {
                item.card.cardObject.SetActive(false);
                item.inHand = false;
            }
        }

    }
    public void SetupCardLocation()
    {
        Vector3 BasePosition = Hand.transform.position;
        float maxSpreadDistance = 50f;
        float maxAngle = 30f;
        float maxHeight = 10f;
        if (hand.Count == 1)
        {
            hand[0].card.cardObject.transform.position = BasePosition + new Vector3(0, 0f, 0f);
            hand[0].card.cardObject.transform.localScale = new Vector3(0.6f, 0.6f);
            hand[0].card.cardObject.transform.parent = Hand.transform;
        }
        else
        {
            for (int i = 0; i < hand.Count; i++)
            {
                float angleHeightInfluence = -Mathf.Abs (((((maxAngle * 2) / hand.Count) * (i + 0.5f)) - maxAngle)/maxAngle);
                hand[i].card.cardObject.transform.position = BasePosition + new Vector3((((maxSpreadDistance * 2) / hand.Count) * (i + 0.5f)) - maxSpreadDistance,
                    angleHeightInfluence*maxHeight, 0f);
                hand[i].card.cardObject.transform.localScale = new Vector3(0.6f, 0.6f);
                hand[i].card.cardObject.transform.parent = Hand.transform;
                hand[i].card.cardObject.transform.rotation = Quaternion.identity;
                hand[i].card.cardObject.transform.Rotate(Vector3.back, (((maxAngle * 2) / hand.Count) * (i + 0.5f)) - maxAngle);
            }
        }
    }
    public void RefreshHand()
    {
        foreach (DeckCard item in hand)
        {
            if (item.card.cardObject == null)
            {
                item.card.CreateCardInstance();
            }
            else
            {
                item.card.ResetCard();
            }
            item.card.cardObject.SetActive(true);
        }
        SetupCardLocation();
    }
}
