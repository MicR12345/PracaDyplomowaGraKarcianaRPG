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

    public void AddCardGFX(Sprite _cardImage, Sprite _cardBorder)
    {
        cardImage = _cardImage;
        cardBorder = _cardBorder;

        
    }
    public void CreateSpriteChildren()
    {
        CreateBorderChild();
        CreateSpriteChild(); 
    }
    public GameObject CreateCardInstance(bool prototype = false)
    {
        if (cardObject==null)
        {
            cardObject = new GameObject(name);
            if (prototype)
            {
                CreateSpriteChildren();
                cardObject.SetActive(false);
                return cardObject;
            }
            else
            {
                CreateSpriteChildren();
                return cardObject;
            }
        }
        else
        {
            Debug.LogError("Card alredy exists");
            return cardObject;
        }
    }
    void CreateSpriteChild()
    {
        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = cardObject.transform;
        spriteObject.transform.localPosition = new Vector3(0f,0.5f,0.5f);
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardImage;
        spriteObject.tag = "card";
    }
    void CreateBorderChild()
    {
        borderObject = new GameObject("Border");
        borderObject.transform.parent = cardObject.transform;
        borderObject.transform.localPosition = Vector3.zero;
        borderRenderer = borderObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = cardBorder;
        borderObject.tag = "card";
        //borderRenderer.sortingOrder = 1;
        borderObject.AddComponent<BoxCollider>();
    }
    public Tag FindCardTag(string name)
    {
        foreach (Tag item in tags)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
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
    public Card(string _name,List<Effect> _effects,List<Tag> _tags)
    {
        name = _name;
        effects = _effects;
        tags = _tags;
    }
    public Card Clone()
    {
        Card card = (Card)this.MemberwiseClone();
        card.cardObject = null;
        card.borderObject = null;
        card.spriteObject = null;
        //card.CreateCardInstance(false);
        card.AddCardGFX(this.cardImage, this.cardBorder);
        //card.cardObject.SetActive(false);
        return card;
    }
    public void DestroyCardInstance()
    {
        GameObject.Destroy(cardObject);
        cardObject = null;
        borderObject = null;
        spriteObject = null;
    }
    public void ResetCard()
    {
        DestroyCardInstance();
        CreateCardInstance(false);
        //cardObject.SetActive(false);
    }
}
public class DeckCard
{
    public bool inHand;
    public bool staysInHand = false;
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
        staysInHand = false;
        usedTimes = 0;
    }
}

public class CardHandle : MonoBehaviour
{
    public DeckCard card;
}