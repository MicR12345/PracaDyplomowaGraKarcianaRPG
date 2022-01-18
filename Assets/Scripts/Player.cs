using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float health;
    public int actionPoints;
    public List<DeckCard> cardStack;
    public List<DeckCard> hand;
    public List<Effect> activeEffects;

    public PlayerData data;

    Sprite playerSprite;

    SpriteRenderer spriteRenderer;

    GameManager gameManager; 
    BattleManager battleManager;

    public GameObject Hand;
    public GameObject PlayerSpriteObject;

    Vector2 playerPosition;
    public void PlayerObjectSetup(BattleManager _battleManager,GameManager _gameManager, Sprite _playerSprite, Vector2 _playerPosition)
    {
        data = new PlayerData();

        this.name = "Player";
        playerSprite = _playerSprite;

        battleManager = _battleManager;
        gameManager = _gameManager;

        playerPosition = _playerPosition;

        CreatePlayerSpriteGO();

        Hand = new GameObject();
        Hand.name = "Hand";
        Hand.transform.parent = this.gameObject.transform;
        Hand.transform.localPosition = new Vector3(0f, -30f, -1f);

        hand = new List<DeckCard>();
        activeEffects = new List<Effect>();
    }
    public void CreatePlayerSpriteGO()
    {
        PlayerSpriteObject = new GameObject("PlayerSprite");
        PlayerSpriteObject.transform.parent = this.transform;
        PlayerSpriteObject.transform.localPosition = playerPosition;
        spriteRenderer = PlayerSpriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerSprite;
    }

    public void PrepareHandBeforeBattle()
    {
        CreateNewCardStack();
    }

    public void AddToPlayerDeck(DeckCard card)
    {
        if (data.deck.Count>data.deckSize)
        {
            Debug.LogError("More cards than possible in deck");
        }
        if (card.card.cardObject == null)
        {
            card.card.CreateCardInstance();
        }
        card.card.cardObject.SetActive(false);
        CardHandle cardHandle = card.card.cardObject.AddComponent<CardHandle>();
        cardHandle.card = card;
        data.deck.Add(card);

    }
    public void CreateNewCardStack(bool random = true)
    {
        cardStack = new List<DeckCard>(data.deck);
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
    public void CheckForCardRemoval()
    {
        List<DeckCard> newHand = new List<DeckCard>();

        foreach (DeckCard item in hand)
        {
            item.card.cardObject.SetActive(false);
            if (!(item.discarded || item.destroyed || item.exhausted>0))
            {
                item.card.cardObject.SetActive(true);
                newHand.Add(item);
            }
        }
        hand = newHand;
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
            Shuffle();
            if (cardStack.Count == 0)
            {
                //w to miejsce trafi tylko gdy graczowi calkowicie skoncza sie karty
                //w tym miejscu jakis efekt ze nie ma wiecej juz kart
            }
        }
    }
    public void DealAFullHand()
    {
        RemoveAllFromHand();
        for (int i = 0; i < data.handSize; i++)
        {
            if (cardStack.Count==0)
            {
                Shuffle();
            }
            if (cardStack.Count > 0)
            {
                AddCardToPlayerHand();
            }      
        }
    }
    public void Shuffle(bool random = true)
    {
        cardStack.Clear();
        foreach (DeckCard item in data.deck)
        {
            item.inHand = false;
        }
        foreach (DeckCard item in hand)
        {
            item.inHand = true;
        }
        foreach (DeckCard item in data.deck)
        {
            if (!item.destroyed && item.exhausted==0 && !item.inHand)
            {
                item.discarded = false;
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
        if (card.card.cardObject==null)
        {
            card.card.CreateCardInstance();
        }
        CardHandle cardHandle = card.card.cardObject.AddComponent<CardHandle>();
        cardHandle.card = card;
        SetupCardLocation();
    }
    public void RemoveAllFromHand()
    {
        foreach (DeckCard item in hand)
        {
            item.discarded = true;
        }
        CheckForCardRemoval();

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
            hand[0].card.cardObject.transform.rotation = Quaternion.identity;
            hand[0].card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f);
            hand[0].card.cardObject.transform.parent = Hand.transform;
        }
        else
        {
            for (int i = 0; i < hand.Count; i++)
            {
                float angleHeightInfluence = -Mathf.Abs (((((maxAngle * 2) / hand.Count) * (i + 0.5f)) - maxAngle)/maxAngle);
                hand[i].card.cardObject.transform.position = BasePosition + new Vector3((((maxSpreadDistance * 2) / hand.Count) * (i + 0.5f)) - maxSpreadDistance,
                    angleHeightInfluence*maxHeight, 0f);
                hand[i].card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f);
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
    public void ApplyCardEffect(Effect effect)
    {
        if (effect.name == "damage")
        {
            float damage = effect.value;
            Effect activeShield = CheckForEffect("shield");
            if (activeShield != null)
            {
                if (damage>=activeShield.value)
                {
                    damage = damage - activeShield.value;
                    RemoveEffect(activeShield);
                }
                else
                {
                    activeShield.value = activeShield.value - damage;
                    damage = 0;
                }
            }
            health = health - damage;
        }
        CheckForDeath();
    }

    public void TakeDamageDirect(float damage)
    {

    }

    public void TickEffects()
    {

    }
    public void CheckForDeath()
    {
        if (health <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public Effect CheckForEffect(string name)
    {
        foreach (Effect item in activeEffects)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
    public void RemoveEffect(Effect effect)
    {
        if (activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
        }
    }
}
