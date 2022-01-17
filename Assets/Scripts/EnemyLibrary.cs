using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyLibrary : MonoBehaviour
{
    public List<Enemy> enemyList;
    GameObject cardLibraryGO;
    CardLibrary cardLibrary;
    public void LoadAllEnemies()
    {
        if (cardLibraryGO == null)
        {
            cardLibraryGO = GameObject.Find("CARD LIBRARY");
            if (cardLibraryGO == null)
            {
                Debug.LogError("Card library object not found");
            }
            else
            {
                if (cardLibrary == null)
                {
                    cardLibrary = cardLibraryGO.GetComponent<CardLibrary>();
                }
                cardLibrary.LoadAllCards();
                Debug.Log("Card library found with " + cardLibrary.cards.Count + " cards.");
            }
        }

        enemyList = new List<Enemy>();
        EnemyLoader enemyLoader = new EnemyLoader("./Assets/Resources/CoreGame/XmlFiles/",cardLibrary);
        enemyList = new List<Enemy>(enemyLoader.enemies);

    }
    [XmlRoot(ElementName = "EnemyList")]
    public class Enemies
    {
        [XmlArray("enemies"), XmlArrayItem("enemy")]
        public List<EnemyData> enemies;
        public Enemies()
        {
            enemies = new List<EnemyData>();
        }
    }
    public class EnemyData
    {
        public string name;
        public float healthMax;
        public float spellDuration;
        [XmlArray("cardSkills"), XmlArrayItem("card")]
        public List<string> cardSkills;
        public string path;

        public EnemyData()
        {
            name = "";
            healthMax = 0;
            spellDuration = 0;
            cardSkills = new List<string>();
            path = "";
        }
    }
    class EnemyLoader
    {
        List<EnemyData> loadedEnemies;
        public List<Enemy> enemies;
        
        public EnemyLoader(string pathtoEnemiesList,CardLibrary cardLibrary)
        {
            loadedEnemies = new List<EnemyData>();
            enemies = new List<Enemy>();
            XmlSerializer reader = new XmlSerializer(typeof(Enemies));
            TextReader enemy = new StreamReader(pathtoEnemiesList + "enemies.xml");
            Enemies loaded = (Enemies)reader.Deserialize(enemy);
            enemy.Close();
            
            foreach(EnemyData i in loaded.enemies)
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D = Resources.Load(i.path + i.name, typeof(Texture2D)) as Texture2D;
                Sprite enemySprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
                Enemy przeciwnik = new Enemy(i.name, i.healthMax, i.spellDuration, i.cardSkills, enemySprite);
                foreach (string item in i.cardSkills)
                {
                    DeckCard deckCard = new DeckCard(cardLibrary.FindCardByName(item));
                    if (deckCard!=null)
                    {
                        przeciwnik.deck.Add(deckCard);
                    }
                }
                enemies.Add(przeciwnik);
            }
        }
    }
}