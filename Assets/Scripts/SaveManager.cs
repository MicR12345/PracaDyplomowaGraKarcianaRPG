using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using System.Xml.Serialization;

public class SaveManager
{
    public static void SaveGame(
        List<WorldMapNode> worldMap,
        int[,] worldDecoratorArray,
        List<WorldMapNode> siegedLocations,
        PlayerData player,
        WorldMapNode playerLocation)
    {
        SaveGameData saveGameData = new SaveGameData(worldMap,worldDecoratorArray,siegedLocations,player,playerLocation);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
        TextWriter writer = new StreamWriter(Application.dataPath + "/save.savegame");
        xmlSerializer.Serialize(writer, saveGameData);
        writer.Close();
    }
    public static SaveGameData LoadGame()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
        TextReader reader = new StreamReader(Application.dataPath + "/save.savegame");
        SaveGameData saveGameData = (SaveGameData)xmlSerializer.Deserialize(reader);
        reader.Close();
        return saveGameData;
    }
    public static void RemoveSavedGame()
    {
        if (File.Exists(Application.dataPath + "/save.savegame"))
        {
            File.Delete(Application.dataPath + "/save.savegame");
        }
    }
    [XmlRoot(ElementName = "SaveData")]
    public class SaveGameData
    {
        public WorldMapData worldMapData;
        public DecoratorArrayData decoratorArray;
        [XmlArray("SiegedLocations"),XmlArrayItem("LocationID")]
        public List<int> siegedLocations;
        public PlayerSaveData playerSaveData;
        public SaveGameData()
        {
            worldMapData = new WorldMapData();
            decoratorArray = new DecoratorArrayData();
            siegedLocations = new List<int>();
            playerSaveData = new PlayerSaveData();
        }
        public SaveGameData(List<WorldMapNode> _worldMap,
        int[,] _worldDecoratorArray,
        List<WorldMapNode> _siegedLocations,
        PlayerData _player,
        WorldMapNode _playerLocation)
        {
            worldMapData = new WorldMapData(_worldMap);
            decoratorArray = new DecoratorArrayData(_worldDecoratorArray);
            siegedLocations = new List<int>();
            foreach (WorldMapNode item in _siegedLocations)
            {
                siegedLocations.Add(_worldMap.IndexOf(item));
            }
            playerSaveData = new PlayerSaveData(_player, _playerLocation, _worldMap);
        }
    }
    public class WorldMapData
    {
        [XmlArray("WorldMap"), XmlArrayItem("WorldMapNode")]
        public List<WorldMapNodeData> worldMapNodes;
        public WorldMapData()
        {
            worldMapNodes = new List<WorldMapNodeData>();
        }
        public WorldMapData(List<WorldMapNode> worldMap)
        {
            worldMapNodes = new List<WorldMapNodeData>();
            foreach (WorldMapNode item in worldMap)
            {
                worldMapNodes.Add(new WorldMapNodeData(item, worldMap));
            }
        }
    }
    public class WorldMapNodeData
    {
        public string name;
        public string type;
        [XmlArray("Connections"), XmlArrayItem("Connection")]
        public List<int> connections;
        public float x;
        public float y;
        public int siegeTime;
        public bool visited;
        public WorldMapNodeData()
        {
            name = "";
            type = "";
            connections = new List<int>();
            x = 0;
            y = 0;
            siegeTime = 0;
            visited = false;
        }
        public WorldMapNodeData(WorldMapNode mapNode,List<WorldMapNode> worldMap)
        {
            name = mapNode.name;
            type = mapNode.type;
            connections = new List<int>();
            foreach (WorldMapNode item in mapNode.connections)
            {
                connections.Add(worldMap.IndexOf(item));
            }
            x = mapNode.gameObject.transform.localPosition.x;
            y = mapNode.gameObject.transform.localPosition.y;
            siegeTime = mapNode.siegeTime;
            visited = mapNode.visited;
        }
    }
    public class DecoratorArrayData
    {
        [XmlArray("DecoratorArrayData"), XmlArrayItem("DecoratorLine")]
        public List<DecoratorArrayLine> decoratorArrayLines;
        public DecoratorArrayData()
        {
            decoratorArrayLines = new List<DecoratorArrayLine>();
        }
        public DecoratorArrayData(int[,] decoArrayData)
        {
            decoratorArrayLines = new List<DecoratorArrayLine>();
            int sizeX = decoArrayData.GetLength(0);
            int sizeY = decoArrayData.GetLength(1);
            for (int y = 0; y < sizeY; y++)
            {
                List<int> decoArrayL = new List<int>();
                for (int x = 0; x < sizeX; x++)
                {
                    decoArrayL.Add(decoArrayData[x, y]);
                }
                DecoratorArrayLine decoratorArrayLine = new DecoratorArrayLine(decoArrayL);
                decoratorArrayLines.Add(decoratorArrayLine);
            }
        }
    }
    public class PlayerSaveData
    {
        public float healthMax;
        public int actionPointsMax;
        public DeckCardsData deck;
        public int deckSize;
        public int handSize;
        public int playerLocation;
        public PlayerSaveData()
        {
            healthMax = 0f;
            actionPointsMax = 0;
            deck = new DeckCardsData();
            deckSize = 0;
            handSize = 0;
            playerLocation = 0;
        }
        public PlayerSaveData(PlayerData player,WorldMapNode playerPosition,List<WorldMapNode> worldMap)
        {
            healthMax = player.healthMax;
            actionPointsMax = player.actionPointsMax;
            deckSize = player.deckSize;
            handSize = player.handSize;
            deck = new DeckCardsData(player.deck);
            playerLocation = worldMap.IndexOf(playerPosition);
        }
    }
    public class DecoratorArrayLine
    {
        [XmlArray("DecoratorData"), XmlArrayItem("Data")]
        public List<int> data;
        public DecoratorArrayLine()
        {
            data = new List<int>();
        }
        public DecoratorArrayLine(List<int> decoArrayLine)
        {
            data = new List<int>(decoArrayLine);
        }
    }
    public class DeckCardsData
    {
        [XmlArray("DeckCards"),XmlArrayItem("DeckCard")]
        public List<DeckCardData> deckCards;
        public DeckCardsData()
        {
            deckCards = new List<DeckCardData>();
        }
        public DeckCardsData(List<DeckCard> _deckCards)
        {
            deckCards = new List<DeckCardData>();
            foreach (DeckCard item in _deckCards)
            {
                deckCards.Add(new DeckCardData(item));
            }
        }
    }
    public class DeckCardData
    {
        public string name;
        public int rarity;
        public int cost;
        [XmlArray("effects"), XmlArrayItem("effect")]
        public List<Effect> effect;
        [XmlArray("tags"), XmlArrayItem("tag")]
        public List<Tag> tag;

        public DeckCardData()
        {
            name = "";
            rarity = 0;
            cost = 0;
            effect = new List<Effect>();
            tag = new List<Tag>();
        }
        public DeckCardData(DeckCard deckCard)
        {
            name = deckCard.card.name;
            rarity = deckCard.card.rarity;
            cost = deckCard.card.cost;
            effect = new List<Effect>(deckCard.card.effects);
            tag = new List<Tag>(deckCard.card.tags);
        }
    }
}
