using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
public class CardLibrary : MonoBehaviour
{

    public List<Card> cards;
    public void LoadAllCards()
    {
        cards = new List<Card>();
        CardLoader cardLoader = new CardLoader("./Assets/Cards/");
        cards = new List<Card>(cardLoader.cards);
        foreach (Card card in cards)
        {
            Debug.Log(card.name);
        }
    }
    public void AddCardPrototype(Card card,Sprite cardImage,Sprite cardBorder)
    {
        card.AddCardGFX(cardImage, cardBorder);
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
    [XmlRoot(ElementName = "CardList")]
    public class Cards
    {
        [XmlArray("cards"), XmlArrayItem("card")]
        public List<CardData> cards;
        public Cards()
        {
            cards=new List<CardData>();
        }
    }

    public class CardData
    {
        public string name;
        public string path;
        public int rarity;
        public int cost;
        [XmlArray("effects"), XmlArrayItem("effect")]
        public List<Effect> effect;
        [XmlArray("tags"), XmlArrayItem("tag")]
        public List<Tag> tag;

        public CardData()
        {
            name = "";
            path = "";
            rarity = 0;
            cost = 0;
            effect = new List<Effect>();
            tag = new List<Tag>();
        }
    }
    class CardLoader
    {
        List<CardData> loadedCards;
        public List<Card> cards;
        List<Sprite> borders;
        public void BordersRarityLoad(string pathToCardBorders)
        {
            borders = new List<Sprite>();
            borders.Add(Resources.Load<Sprite>("./CardsGFX/0_cardTest"));
        }
        public CardLoader(string pathToCardList)
        {
            BordersRarityLoad("");
            loadedCards = new List<CardData>();
            cards = new List<Card>();
            XmlSerializer reader  = new XmlSerializer(typeof(Cards));
            TextReader card = new StreamReader(pathToCardList + "cards.xml");
            Cards loaded = (Cards)reader.Deserialize(card);

            card.Close();
            foreach(CardData i in loaded.cards)
            {
                Card karta = new Card(i.name, i.effect, i.tag);
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D = Resources.Load(i.path + i.name, typeof(Texture2D)) as Texture2D;
                Object stary = Resources.Load(i.path + i.name);
                Object nowy = stary;
                Sprite cardImage = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
                karta.AddCardGFX(cardImage, borders[i.rarity]);
                Debug.Log(i.path + i.name + ".png");
                cards.Add(karta);

            }
        }
    }
}