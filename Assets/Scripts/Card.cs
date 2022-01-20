using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Card : Action
{
    public GameObject cardObject;
    public FancyCardMover cardMover;

    Sprite cardImage;
    Sprite cardBorder;

    SpriteRenderer spriteRenderer;
    SpriteRenderer borderRenderer;

    GameObject spriteObject;
    GameObject borderObject;
    GameObject uiGameObject;
    GameObject descriptionTextGameObject;

    TextMeshPro descriptionText;

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
    public void CreateUI()
    {
        CreateUIObject();
        CreateDescriptionText();
    }
    public GameObject CreateCardInstance(bool prototype = false)
    {
        cardObject = new GameObject(name);
        cardMover = cardObject.AddComponent<FancyCardMover>();
        CreateSpriteChildren();
        CreateUI();
        UpdateDescription();
        return cardObject;
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
        borderRenderer.sortingOrder = 1;
        borderObject.AddComponent<BoxCollider>();
    }
    GameObject CreateUIObject()
    {
        uiGameObject = new GameObject("UI Canvas");
        uiGameObject.transform.parent = cardObject.transform;
        uiGameObject.transform.localPosition = new Vector3(0f,-10f,0f);
        uiGameObject.AddComponent<Canvas>();
        RectTransform canvasRT = uiGameObject.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(25f, 18f);
        return uiGameObject;
    }
    GameObject CreateDescriptionText()
    {
        descriptionTextGameObject = new GameObject("Description Text");
        descriptionTextGameObject.transform.parent = uiGameObject.transform;
        descriptionTextGameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        descriptionText = descriptionTextGameObject.AddComponent<TextMeshPro>();
        descriptionText.horizontalAlignment = HorizontalAlignmentOptions.Center;
        descriptionText.verticalAlignment = VerticalAlignmentOptions.Top;
        descriptionText.sortingOrder = 15;
        descriptionText.color = Color.black;
        descriptionText.text = "Something went wrong";
        RectTransform rectTransform = descriptionTextGameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(25f, 18f);
        return descriptionTextGameObject;
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
        string newDescription = "";
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        foreach (Effect item in effects)
        {
            newDescription = newDescription + textInfo.ToTitleCase(item.name);
            if (item.level!=0)
            {
                newDescription = newDescription + "+" + item.level;
            }
            if (item.value!=0)
            {
                newDescription = newDescription + " " + item.value;
            }
            newDescription = newDescription + "\n";
        }
        foreach (Tag item in tags)
        {
            newDescription = newDescription + textInfo.ToTitleCase(item.name) + "\n";
        }
        description = newDescription;
        descriptionText.text = description;
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