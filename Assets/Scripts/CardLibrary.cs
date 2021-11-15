using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class CardLibrary : MonoBehaviour
{
    public Texture2D LoadCardImg(string name)
    {
        Texture2D texture = new Texture2D(320, 480);
        byte[] byteData;

        string path = "./Assets/Graphics/Cards/" + name + ".png";

        if (File.Exists(path))
        {
            byteData = File.ReadAllBytes(path);
            texture.LoadImage(byteData);
        }
        return texture;
    }

    public List<CardObject> cards;
    public void LoadAllCards()
    {
        cards = new List<CardObject>();
        Card card = new Card("0_cardTest", 1);
        Texture2D texture2D = LoadCardImg(card.name);
        Sprite sprite = Sprite.Create(texture2D,new Rect(0.0f,0.0f,texture2D.width,texture2D.height), new Vector2(0.5f, 0.5f),15f);
        AddCardPrototype(card,sprite,null);
    }
    public void AddCardPrototype(Card card,Sprite cardImage,Sprite cardBorder)
    {
        GameObject cardObject = new GameObject(card.name);
        CardObject newCard = cardObject.AddComponent<CardObject>();
        cardObject.transform.parent = this.transform;
        cardObject.transform.localPosition = Vector3.zero;
        newCard.SetupCardPrototype(cardObject, cardImage, cardBorder, card);
        cards.Add(newCard);
    }
    public CardObject FindCardByName(string name)
    {
        foreach (CardObject item in cards)
        {
            Debug.Log("Compared " + item.card.name + " to " + name);
            if (item.card.name == name) return item;
        }
        return null;
    }
    public void DebugPrintAllCardsNames()
    {
        foreach (CardObject item in cards)
        {
            Debug.Log(item.card.name);
        }
    }
}
