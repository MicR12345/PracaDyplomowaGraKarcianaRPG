using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card : Action
{
    public GameObject cardObject;

    Sprite cardImage;
    Sprite cardBorder;

    SpriteRenderer spriteRenderer;
    SpriteRenderer borderRenderer;

    GameObject spriteObject;
    GameObject borderObject;

    BoxCollider2D boxCollider;
    public void SetupCardPrototype(Sprite _cardImage, Sprite _cardBorder)
    {
        cardImage = _cardImage;
        cardBorder = _cardBorder;

        CreateSpriteChild();
        //CreateBorderChild();
    }
    public GameObject CreateCardInstance(bool prototype = false)
    {
        if (cardObject==null)
        {
            cardObject = new GameObject(name);
            if (prototype)
            {
                cardObject.SetActive(false);
                return cardObject;
            }
        }
        else
        {
            Debug.LogError("Card alredy exists");
            return cardObject;
        }
        return null;
    }
    void CreateSpriteChild()
    {
        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = cardObject.transform;
        spriteObject.transform.localPosition = Vector3.zero;
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardImage;
        spriteObject.tag = "card_sprite";
        spriteObject.AddComponent<BoxCollider>();
    }
    void CreateBorderChild()
    {
        borderObject = new GameObject("Border");
        borderObject.transform.parent = cardObject.transform;
        borderObject.transform.localPosition = Vector3.zero;
        borderRenderer = spriteObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = cardBorder;
    }
    public override void UpdateDescription()
    {
        throw new System.NotImplementedException();
    }
    public override void UpgradeCard()
    {
        throw new System.NotImplementedException();
    }
    public override bool CanUpgrade()
    {
        throw new System.NotImplementedException();
    }
    public Card(string _name,int _damage)
    {
        name = _name;
        damage = _damage;
    }
    public Card Clone()
    {
        Card card = (Card)this.MemberwiseClone();
        card.cardObject = null;
        card.borderObject = null;
        card.spriteObject = null;
        card.CreateCardInstance(false);
        card.SetupCardPrototype(this.cardImage, this.cardBorder);
        return card;
    }
}
public class DeckCard
{
    public bool discarded;
    public int exhausted;
    public bool destroyed;
    public int usedTimes;
    public Card card;
    public DeckCard(Card _card)
    {
        card = _card.Clone();
        discarded = false;
        exhausted = 0;
        destroyed = false;
        usedTimes = 0;
    }
}