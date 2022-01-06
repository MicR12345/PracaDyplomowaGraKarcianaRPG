using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using UnityEngine;
//Do zrobienia ładowanie kart z xml
public class CardLibrary : MonoBehaviour
{

    public List<Card> cards;
    public void LoadAllCards()
    {
        cards = new List<Card>();
        /* CreateDebugCard("0_cardTest", 1);
         CreateDebugCard("cardTest2", 1);
         CreateDebugCard("cardTest3", 1);*/
        CardLoader cardLoader = new CardLoader("./Assets/Cards/");
        cards = new List<Card>(cardLoader.cards);
        foreach (Card card in cards)
        {
            Debug.Log(card.name);
        }
    }
    /*
    void CreateDebugCard(string name,int damage)
    {
        List<Effect> effects = new List<Effect>();
        effects.Add(new Effect("damage",-1,3,-1,new List<Tag>()));
        List<Tag> tags = new List<Tag>();
        tags.Add(new Tag("discard", -1));
        Card card = new Card(name ,effects,tags);
        Texture2D texture2D = LoadCardImg(card.name);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
        AddCardPrototype(card, sprite, null);
    }*/
    public void AddCardPrototype(Card card,Sprite cardImage,Sprite cardBorder)
    {
        //GameObject cardObject = card.CreateCardInstance(true);
        //cardObject.transform.parent = this.transform;
        //cardObject.tag = "Card";
        //cardObject.transform.localPosition = Vector3.zero;
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
        [XmlElement("name")]
        public string name;
        [XmlElement("path")]
        public string path;
        [XmlElement("rarity")]
        public int rarity;
        [XmlElement("cost")]
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
            Texture2D texture2D = LoadImg("0_cardTest", "./Assets/CardsGFX/");
            borders.Add(Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f));
        }
        public CardLoader(string pathToCardList)
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            // Set a new Namespace and ElementName for the root element.
            xRoot.Namespace = "";
            xRoot.ElementName = "cards";
            BordersRarityLoad("");
            loadedCards = new List<CardData>();
            cards = new List<Card>();
            XmlSerializer reader  = new XmlSerializer(typeof(Cards), xRoot);
            TextReader card = new StreamReader(pathToCardList + "cards.xml");
            Cards loaded = (Cards)reader.Deserialize(card);
            card.Close();
            foreach(CardData i in loaded.cards)
            {
                Card karta = new Card(i.name, i.effect, i.tag);
                Texture2D cardTexture = LoadImg(i.name, i.path);
                karta.AddCardGFX(Sprite.Create(cardTexture, new Rect(0.0f, 0.0f, cardTexture.width, cardTexture.height), new Vector2(0.5f, 0.5f), 15f),borders[i.rarity]);
                cards.Add(karta);
            }
        }

        public Texture2D LoadImg(string name, string pathToImage)
        {
            Texture2D texture = new Texture2D(320, 480);
            byte[] byteData;

            string path = pathToImage + name + ".png";

            if (File.Exists(path))
            {
                byteData = File.ReadAllBytes(path);
                texture.LoadImage(byteData);
            }
            return texture; 
        }
    }
}