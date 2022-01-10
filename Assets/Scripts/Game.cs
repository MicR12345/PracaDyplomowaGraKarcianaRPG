using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    static int villageChance = 30;
    static int cityChance = 20;
    static int fortressChance = 5;
    static int acientRuinsChance = 5;

    List<WorldMapNode> worldMap;
    string playerPosition;
    void StartSiege(string nodeName)
    {

    }
    void MovePlayer(string nodeName)
    {

    }
    void LaunchEvent(string eventName)
    {

    }
    void BeginCombat(List<Enemy> enemies,PlayerData playerData/*,location?*/)
    {

    }
    void GenerateWorldMap(int nodeCount)
    {
        worldMap = new List<WorldMapNode>();

        worldMap.Add(new WorldMapNode("EnemyCastle", "EnemyCastle"));
        worldMap.Add(new WorldMapNode("Capitol", "Capitol"));

        int maxChance = villageChance + cityChance + fortressChance + acientRuinsChance;
        for (int i = 1; i < nodeCount; i++)
        {
            int roll = UnityEngine.Random.Range(0, maxChance);
            if (roll < villageChance)
            {
                worldMap.Add(new WorldMapNode("Village", "Village"));
            }
            else if (roll >= villageChance && roll<villageChance + cityChance)
            {
                worldMap.Add(new WorldMapNode("City", "City"));
            }
            else if (roll >= villageChance + cityChance && roll < villageChance + cityChance + fortressChance)
            {

            }
            else if (roll >= villageChance + cityChance + fortressChance && roll < villageChance + cityChance + fortressChance + acientRuinsChance)
            {

            }
        }
    }
    void DrawWorldMap()
    {

    }

}
public class WorldMapNode
{
    string name;
    string type;
    List<string> connections;
    string state;
    int siegeTime;
    public WorldMapNode(string _name,string _type)
    {
        name = _name;
        type = _type;
        connections = new List<string>();
        state = "normal";
        siegeTime = 0;
    }
}