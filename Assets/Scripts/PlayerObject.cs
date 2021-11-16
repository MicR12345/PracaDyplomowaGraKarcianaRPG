using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    private Player player;
    Sprite playerSprite;

    SpriteRenderer spriteRenderer;
    Material spriteMaterial;

    GameManager gm;

    public GameObject Hand;
    List<GameObject> cardsInHand;
    public void PlayerObjectSetup(GameManager _gm,Player _player, Sprite _playerSprite,Material _spriteMaterial)
    {
        this.name = "Player";
        player = _player;
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
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCardToPlayerHand(Card card)
    {
        player.hand.Add(card);
        SetupCardLocation();
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
        foreach (Card item in player.hand)
        {
            CardObject cardPrototype = gm.cardLibrary.FindCardByName(item.name);
            if (cardPrototype != null)
            {
                cardPrototype.card = item;
                GameObject newCard = Instantiate(cardPrototype.gameObject, Hand.transform);
                newCard.GetComponent<CardObject>().CreateCard();
                cardsInHand.Add(newCard);
            }
            else
            {
                Debug.LogError("Card not found in library \"" + item.name + "\"");
            }
        }
    }
}
