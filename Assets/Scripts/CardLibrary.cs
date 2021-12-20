using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
//Do zrobienia ładowanie kart z xml
public class CardLibrary : MonoBehaviour
{
    public Texture2D LoadCardImg(string name)
    {
        Texture2D texture = new Texture2D(320, 480);
        byte[] byteData;

        string path = "./Assets/Cards/CardGFX/" + name + ".png";

        if (File.Exists(path))
        {
            byteData = File.ReadAllBytes(path);
            texture.LoadImage(byteData);
        }
        return texture;
    }

    public List<Card> cards;
    public void LoadAllCards()
    {
        cards = new List<Card>();
        CreateDebugCard("0_cardTest", 1);
        CreateDebugCard("cardTest2", 1);
        CreateDebugCard("cardTest3", 1);
    }
    void CreateDebugCard(string name,int damage)
    {
        Card card = new Card(name, damage);
        Texture2D texture2D = LoadCardImg(card.name);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
        AddCardPrototype(card, sprite, null);
    }
    public void AddCardPrototype(Card card,Sprite cardImage,Sprite cardBorder)
    {
        //GameObject cardObject = card.CreateCardInstance(true);
        //cardObject.transform.parent = this.transform;
        //cardObject.tag = "Card";
        //cardObject.transform.localPosition = Vector3.zero;
        card.SetupCardPrototype(cardImage, cardBorder);
        cards.Add(card);
    }
    public Card FindCardByName(string name)
    {
        foreach (Card item in cards)
        {
            Debug.Log("Compared " + item.name + " to " + name);
            if (item.name == name) return item;
        }
        return null;
    }
    public void DebugPrintAllCardsNames()
    {
        foreach (Card item in cards)
        {
            Debug.Log(item.name);
        }
    }
    class CardLoader
    {
        List<Card> loadedCards;
        public CardLoader(string pathToCardList)
        {

        }
    }
}
