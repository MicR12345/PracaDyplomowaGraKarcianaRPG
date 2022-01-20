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
        EnemyLoader enemyLoader = new EnemyLoader("/CoreGame/XmlFiles/enemies.xml",cardLibrary);
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
        public int frameCount;

        public EnemyData()
        {
            name = "";
            healthMax = 0;
            spellDuration = 0;
            cardSkills = new List<string>();
            path = "";
            frameCount = 0;
        }
    }
    class EnemyLoader
    {
        List<EnemyData> loadedEnemies;
        public List<Enemy> enemies;
        
        public EnemyLoader(string pathToEnemiesList,CardLibrary cardLibrary)
        {
            loadedEnemies = new List<EnemyData>();
            enemies = new List<Enemy>();
            string enemyList = File.ReadAllText(Application.dataPath + pathToEnemiesList);
            XmlSerializer reader = new XmlSerializer(typeof(Enemies));
            TextReader enemy = new StringReader(enemyList);
            Enemies loaded = (Enemies)reader.Deserialize(enemy);
            enemy.Close();
            
            foreach(EnemyData i in loaded.enemies)
            {
                Texture2D texture2D = new Texture2D(1, 1);
                byte[] bytes = File.ReadAllBytes(Application.dataPath + "/" + i.path + i.name + ".png");
                texture2D.LoadImage(bytes);
                texture2D.filterMode = FilterMode.Point;
                List<Sprite> enemySprites = new List<Sprite>();
                float singleTextureSize = texture2D.width / i.frameCount;
                for (int j = 0; j < i.frameCount; j++)
                {
                    enemySprites.Add(Sprite.Create(texture2D, new Rect(j * singleTextureSize, 0.0f,singleTextureSize, texture2D.height), new Vector2(0.5f, 0.5f), 6f));
                }
                Enemy przeciwnik = new Enemy(i.name, i.healthMax, i.spellDuration, i.cardSkills, enemySprites);
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
