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
        Sprite sprite = Sprite.Create(texture2D,new Rect(0.0f,0.0f,texture2D.width,texture2D.height), new Vector2(0.5f, 0.5f));
        cards.Add(new CardObject(sprite, sprite, card));
    }
}
