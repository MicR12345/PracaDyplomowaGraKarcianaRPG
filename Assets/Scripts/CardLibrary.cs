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
        CardLoader cardLoader = new CardLoader("/CoreGame/XmlFiles/cards.xml");
        cards = new List<Card>(cardLoader.cards);
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
            if (item.name == name) return item;
        }
        return null;
    }
    public Card FindRandomCardByRarity(int rarity)
    {
        List<Card> randomList = new List<Card>();
        foreach (Card item in cards)
        {
            if (item.rarity == rarity) randomList.Add(item);
        }
        if (randomList.Count == 0)
        {
            Debug.LogError("No cards of rarity " + rarity + " found");
            return null;
        }
        int random = UnityEngine.Random.Range(0,randomList.Count);
        return randomList[random];
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
        List<CardData> loadedCards;
        public List<Card> cards;
        List<Sprite> borders;
        public void BordersRarityLoad(string pathToCardBorders)
        {
            List<Texture2D> borderTextures = new List<Texture2D>();
            foreach (string item in Directory.GetFiles(Application.dataPath + pathToCardBorders,"*.png"))
            {
                byte[] bytes = File.ReadAllBytes(item);
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(bytes);
                borderTextures.Add(texture2D);
            }
            borders = new List<Sprite>();
            foreach (Texture2D item in borderTextures)
            {
                item.filterMode = FilterMode.Point;
                Sprite border = Sprite.Create(item, new Rect(0.0f, 0.0f, item.width, item.height), new Vector2(0.5f, 0.5f), 5f);
                borders.Add(border);
            }
        }
        public CardLoader(string pathToCardList)
        {
            string cardList = File.ReadAllText(Application.dataPath + pathToCardList);
            //TextAsset cardXml = Resources.Load(pathToCardList, typeof(TextAsset)) as TextAsset;
            BordersRarityLoad("/CoreGame/BordersGFX/");
            loadedCards = new List<CardData>();
            cards = new List<Card>();
            XmlSerializer reader  = new XmlSerializer(typeof(Cards));
            TextReader card = new StringReader(cardList);
            Cards loaded = (Cards)reader.Deserialize(card);
            card.Close();
            foreach(CardData i in loaded.cards)
            {
                Card karta = new Card(i.name,i.cost,i.rarity , i.effect, i.tag);
                Texture2D texture2D = new Texture2D(1, 1);
                byte[] bytes;
                if (File.Exists(Application.dataPath + "/" + i.path + i.name + ".png"))
                {
                    bytes = File.ReadAllBytes(Application.dataPath + "/" + i.path + i.name + ".png");
                }
                else
                {
                    bytes = File.ReadAllBytes(Application.dataPath + "/" + i.path + "0_cardTest" + ".png");
                }
                
                texture2D.LoadImage(bytes);
                texture2D.filterMode = FilterMode.Point;
                Sprite cardImage = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
                karta.AddCardGFX(cardImage, borders[i.rarity]);
                cards.Add(karta);

            }
        }
    }
}
[XmlRoot(ElementName = "CardList")]
public class Cards
{
    [XmlArray("cards"), XmlArrayItem("card")]
    public List<CardData> cards;
    public Cards()
    {
        cards = new List<CardData>();
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