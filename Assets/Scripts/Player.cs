using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{
    public float health;
    public int actionPoints;
    public List<DeckCard> cardStack;
    public List<DeckCard> hand;
    public List<Effect> activeEffects;

    public PlayerData data;

    List<Sprite> playerSprites;

    SpriteRenderer spriteRenderer;
    public SpriteAnimator spriteAnimator;

    GameManager gameManager; 
    BattleManager battleManager;

    public GameObject Hand;
    public GameObject playerSpriteObject;

    public GameObject uiGameObject;
    public GameObject hpTextGameObject;
    public GameObject apTextGameObject;

    public TextMeshPro hpText;
    public TextMeshPro apText;

    public GameObject initiativeBar;

    public Slider initiativeSlider;

    Vector2 playerPosition;
    public void PlayerObjectSetup(BattleManager _battleManager,GameManager _gameManager, List<Sprite> _playerSprites, Vector2 _playerPosition)
    {

        this.name = "Player";
        playerSprites = _playerSprites;

        battleManager = _battleManager;
        gameManager = _gameManager;
        data = _gameManager.player;
        playerPosition = _playerPosition;

        CreatePlayerSpriteGO();

        Hand = new GameObject();
        Hand.name = "Hand";
        Hand.transform.parent = this.gameObject.transform;
        Hand.transform.localPosition = new Vector3(0f, -30f, -1f);

        hand = new List<DeckCard>();
        activeEffects = new List<Effect>();
        CreateUIElements();
    }
    public void CreatePlayerSpriteGO()
    {
        playerSpriteObject = new GameObject("PlayerSprite");
        playerSpriteObject.transform.parent = this.transform;
        playerSpriteObject.transform.localPosition = playerPosition;
        spriteRenderer = playerSpriteObject.AddComponent<SpriteRenderer>();
        spriteAnimator = playerSpriteObject.AddComponent<SpriteAnimator>();
        spriteAnimator.setupSprites(spriteRenderer, playerSprites,"idle",1f);
        spriteAnimator.startAnimation();
    }
    GameObject CreateUIObject()
    {
        uiGameObject = new GameObject("UI Canvas");
        uiGameObject.transform.parent = playerSpriteObject.transform;
        uiGameObject.transform.localPosition = Vector3.zero;
        uiGameObject.AddComponent<Canvas>();
        RectTransform canvasRT = uiGameObject.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(2.30f, 0.25f);
        return uiGameObject;
    }
    GameObject CreateHPText()
    {
        hpTextGameObject = new GameObject("HP Text");
        hpTextGameObject.transform.parent = uiGameObject.transform;
        hpTextGameObject.transform.localPosition = new Vector3(0f, -10f, 0f);
        hpText = hpTextGameObject.AddComponent<TextMeshPro>();
        hpText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        hpText.verticalAlignment = VerticalAlignmentOptions.Middle;
        hpText.text = "0/0";
        return hpTextGameObject;
    }
    GameObject CreateAPText()
    {
        apTextGameObject = new GameObject("AP Text");
        apTextGameObject.transform.parent = uiGameObject.transform;
        apTextGameObject.transform.localPosition = new Vector3(0f, -1f, 0f);
        apText = apTextGameObject.AddComponent<TextMeshPro>();
        apText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        apText.verticalAlignment = VerticalAlignmentOptions.Middle;
        apText.text = "0/0";
        return apTextGameObject;
    }
    GameObject CreateCardBar()
    {
        initiativeBar = new GameObject("Card Bar");
        initiativeBar.transform.parent = Hand.transform;
        initiativeBar.transform.localPosition = new Vector3(0f, -20f);
        initiativeBar.AddComponent<Canvas>();
        RectTransform rectTransformSize = initiativeBar.GetComponent<RectTransform>();
        rectTransformSize.sizeDelta = new Vector2(20f, 1f);
        GameObject sliderObject = new GameObject("Slider");
        sliderObject.transform.parent = initiativeBar.transform;
        initiativeSlider = sliderObject.AddComponent<Slider>();
        GameObject sliderBar = new GameObject("Slider Bar");
        sliderBar.transform.parent = initiativeBar.transform;
        Image image = sliderBar.AddComponent<Image>();
        RectTransform rectTransform = sliderBar.GetComponent<RectTransform>();

        initiativeSlider.fillRect = rectTransform;
        rectTransform.sizeDelta = new Vector2(0f, 0f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        return initiativeBar;
    }
    void CreateUIElements()
    {
        CreateUIObject();
        CreateHPText();
        CreateCardBar();
        CreateAPText();
    }
    public void PrepareHandBeforeBattle()
    {
        CreateNewCardStack();
        CreateInstancesOfCards();
    }
    public void CreateInstancesOfCards()
    {
        foreach (DeckCard item in data.deck)
        {
            item.card.CreateCardInstance();
            item.card.cardObject.SetActive(false);
            CardHandle cardHandle = item.card.cardObject.AddComponent<CardHandle>();
            cardHandle.card = item;
        }
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
                SetupCardLocation();
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
        SetupCardLocation();
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
            hand[0].card.cardMover.moveCardTo = transform.TransformPoint(BasePosition + new Vector3(0, 0f, 0f));
            hand[0].card.cardMover.rotateTo = Vector3.zero;
            hand[0].card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f);
            hand[0].card.cardObject.transform.parent = Hand.transform;
        }
        else
        {
            for (int i = 0; i < hand.Count; i++)
            {
                float angleHeightInfluence = -Mathf.Abs (((((maxAngle * 2) / hand.Count) * (i + 0.5f)) - maxAngle)/maxAngle);
                hand[i].card.cardMover.moveCardTo = transform.TransformPoint(BasePosition + new Vector3((((maxSpreadDistance * 2) / hand.Count) * (i + 0.5f)) - maxSpreadDistance,
                    angleHeightInfluence*maxHeight, 0f));
                hand[i].card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f);
                hand[i].card.cardObject.transform.parent = Hand.transform;
                hand[i].card.cardMover.rotateTo = new Vector3(0f,0f,-((((maxAngle * 2) / hand.Count) * (i + 0.5f)) - maxAngle));
                hand[i].card.SetSortingOrder(i * 2);
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
            battleManager.pointerControl.UnRegisterFromInput();
            gameManager.LoseGame();
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
    public void UpdateHpBar()
    {
        hpText.text = health + "/" + data.healthMax;
    }
    public void UpdateApDisplay()
    {
        apText.text = actionPoints + "/" + data.actionPointsMax;
    }
    public void UpdateCardBar(float amount)
    {
        initiativeSlider.value = 1 - (amount / BattleManager.playerCardGiveTime);
    }
}
