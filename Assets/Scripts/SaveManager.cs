using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class SaveManager
{
    public List<WorldMapNode> worldMap;
    public int[,] worldDecoratorArray;
    public List<WorldMapNode> siegedLocations;
    public PlayerData player;
    public WorldMapNode playerLocation;

    public void SaveGame(
        List<WorldMapNode> worldMap,
        int[,] worldDecoratorArray,
        List<WorldMapNode> siegedLocations,
        PlayerData player,
        WorldMapNode playerLocation)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
    }
    [XmlRoot(ElementName = "SaveData")]
    public class SaveGameData
    {
        public WorldMapData worldMapData;
        public SaveGameData()
        {

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
    }
    public class WorldMapNodeData
    {
        public string name;
        public string type;
        [XmlArray("Connections"), XmlArrayItem("Connection")]
        public List<int> connections;
        public float x;
        public float y;
        public float z;
        public int siegeTime;
        public bool visited;
    }
}
