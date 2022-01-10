using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static int worldMapSize = 30;

    static int villageChance = 30;
    static int cityChance = 20;
    static int fortressChance = 5;
    static int acientRuinsChance = 5;

    static int maxConnectionCount = 5;
    static int maxConnectAttempts = 10;

    static float worldMapLocationPullForce = 0.2f;
    static float smoothingCount = 3;

    static float minSmoothingDistance = 8f;

    static Vector3 worldBoundsStart = new Vector3(-30f,-25f,0f);
    static Vector3 worldBoundsEnd = new Vector3(30f, 25f, 0f);

    List<WorldMapNode> worldMap;
    string playerPosition;

    public Sprite enemyCastleSprite;
    public Sprite capitolSprite;
    public Sprite citySprite;
    public Sprite villageSprite;
    public Sprite fortressSprite;
    public Sprite ruinsSprite;
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

        worldMap.Add(new WorldMapNode(this,"EnemyCastle", "EnemyCastle"));
        worldMap[0].SetPosition(worldBoundsEnd);
        worldMap.Add(new WorldMapNode(this, "Capitol", "Capitol"));

        int maxChance = villageChance + cityChance + fortressChance + acientRuinsChance;
        for (int i = 1; i < nodeCount; i++)
        {
            int roll = UnityEngine.Random.Range(0, maxChance);
            if (roll < villageChance)
            {
                worldMap.Add(new WorldMapNode(this, "Village", "Village"));
            }
            else if (roll >= villageChance && roll<villageChance + cityChance)
            {
                worldMap.Add(new WorldMapNode(this, "City", "City"));
            }
            else if (roll >= villageChance + cityChance && roll < villageChance + cityChance + fortressChance)
            {
                worldMap.Add(new WorldMapNode(this, "Fortress", "Fortress"));
            }
            else if (roll >= villageChance + cityChance + fortressChance && roll < villageChance + cityChance + fortressChance + acientRuinsChance)
            {
                worldMap.Add(new WorldMapNode(this, "Ruins", "AcientRuins"));
            }
        }
        int fortressCountNearEnemy = UnityEngine.Random.Range(3, maxConnectionCount);
        for (int i = 0; i < fortressCountNearEnemy; i++)
        {
            int worldNodeCount = worldMap.Count;

            worldMap.Add(new WorldMapNode(this, "BorderFortress", "Fortress"));
            worldMap[0].connections.Add(worldMap[worldNodeCount]);
            worldMap[worldNodeCount].connections.Add(worldMap[0]);
        }
        for (int i = 1; i < worldMap.Count; i++)
        {
            //Do zrobienia polaczenia z najlbizszymi od 1 do n nodow zamiast tego
            ConnectToRandomNodes(worldMap[i]);
            worldMap[i].PlaceRandomlyWithinBounds(worldBoundsStart, worldBoundsEnd);
        }
        SmoothLocationPositions();
    }
    void DrawWorldMap()
    {

    }
    void ConnectToRandomNodes(WorldMapNode node)
    {
        for (int i = 0; i < maxConnectAttempts && node.connections.Count<maxConnectionCount; i++)
        {
            int randomNodeNumber = UnityEngine.Random.Range(1, worldMap.Count);
            if (randomNodeNumber != i && worldMap[randomNodeNumber].connections.Count<maxConnectionCount)
            {
                node.connections.Add(worldMap[randomNodeNumber]);
                worldMap[randomNodeNumber].connections.Add(node);
            }
        }
    }
    void SmoothLocationPositions()
    {
        for (int i = 0; i < smoothingCount-1; i++)
        {
            List<Vector3> positions = PrepareListOfNewPositions();
            for (int j = 0; j < worldMap.Count; j++)
            {
                worldMap[j].SetPosition(positions[j]);
            }
            NormalizeNodesToBounds();
        }
        List<Vector3> positionsFinal = PrepareListOfNewPositions();
        for (int j = 0; j < worldMap.Count; j++)
        {
            worldMap[j].SetPosition(positionsFinal[j]);
        }
    }
    void NormalizeNodesToBounds()
    {
        float minX = worldBoundsEnd.x;
        float minY = worldBoundsEnd.y;
        float maxX = worldBoundsStart.x;
        float maxY = worldBoundsStart.y;
        for (int i = 0; i < worldMap.Count; i++)
        {
            float x = worldMap[i].gameObject.transform.position.x;
            float y = worldMap[i].gameObject.transform.position.y;
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
        }
        for (int i = 0; i < worldMap.Count; i++)
        {
            float x = worldMap[i].gameObject.transform.position.x;
            float y = worldMap[i].gameObject.transform.position.y;
            if (x>0)
            {
                x = (x / maxX) * worldBoundsEnd.x;
            }
            else
            {
                x = (x / minX) * worldBoundsStart.x;
            }

            if (y > 0)
            {
                y = (y / maxY) * worldBoundsEnd.y;
            }
            else
            {
                y = (y / minY) * worldBoundsStart.y;
            }
            Vector3 normalizedPosition = new Vector3(x,y,0f);
            worldMap[i].SetPosition(normalizedPosition);
        }
    }
    List<Vector3> PrepareListOfNewPositions()
    {
        List<Vector3> newPositions = new List<Vector3>();
        for (int i = 0; i < worldMap.Count; i++)
        {
            newPositions.Add(CalculateNodePosition(worldMap[i], worldMapLocationPullForce));
        }
        return newPositions;
    }
    Vector3 CalculateNodePosition(WorldMapNode node,float pullForce)
    {
        Vector3 averageOfConnections = CalculateAveragePositionBetweenConnections(node);
        float xDistance = averageOfConnections.x - node.gameObject.transform.localPosition.x;
        float yDistance = averageOfConnections.y - node.gameObject.transform.localPosition.y;
        return node.gameObject.transform.localPosition + new Vector3(xDistance * pullForce, yDistance * pullForce, 0f);
    }
    Vector3 CalculateAveragePositionBetweenConnections(WorldMapNode node)
    {
        float averageX = 0;
        float averageY = 0;
        float weightSum = 0;
        foreach (WorldMapNode item in node.connections)
        {
            float distance = Vector3.Distance(node.gameObject.transform.localPosition, item.gameObject.transform.localPosition);
            if (Vector3.Distance(node.gameObject.transform.localPosition, item.gameObject.transform.localPosition) >minSmoothingDistance)
            {
                averageX = averageX + item.gameObject.transform.localPosition.x * distance;
                averageY = averageY + item.gameObject.transform.localPosition.y * distance;
            }
            else
            {
                averageX = averageX - item.gameObject.transform.localPosition.x * distance;
                averageY = averageY - item.gameObject.transform.localPosition.y * distance;
            }
            weightSum = weightSum + distance;
        }
        return new Vector3(averageX / weightSum, averageY / weightSum, 0f);
    }

    private void Start()
    {
        if (worldMap == null)
        {
            GenerateWorldMap(worldMapSize);
        }
    }
    private void OnDrawGizmos()
    {
        if (worldMap !=null)
        {
            foreach (WorldMapNode item in worldMap)
            {
                foreach (WorldMapNode connection in item.connections)
                {
                    Gizmos.DrawLine(item.gameObject.transform.position, connection.gameObject.transform.position);
                }
            }
        }
    }
}
public class WorldMapNode
{
    GameManager gameManager;

    public string name;
    public string type;
    public List<WorldMapNode> connections;
    public string state;

    public GameObject gameObject;

    GameObject spriteObject;
    SpriteRenderer spriteRenderer;

    int siegeTime;
    public WorldMapNode(GameManager _gameManager,string _name,string _type)
    {
        gameManager = _gameManager;

        name = _name;
        type = _type;
        connections = new List<WorldMapNode>();
        state = "normal";
        siegeTime = 0;

        gameObject = new GameObject(name);
        gameObject.transform.parent = gameManager.gameObject.transform;
        gameObject.transform.localPosition = Vector3.zero;

        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = gameObject.transform;
        spriteObject.transform.localPosition = Vector3.zero;

        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        

        SetSprite();
    }

    void SetSprite()
    {
        if (type == "EnemyCastle")
        {
            spriteRenderer.sprite = gameManager.enemyCastleSprite;
        }
        else if (type == "Capitol")
        {
            spriteRenderer.sprite = gameManager.capitolSprite;
        }
        else if (type == "City")
        {
            spriteRenderer.sprite = gameManager.citySprite;
        }
        else if (type == "Village")
        {
            spriteRenderer.sprite = gameManager.villageSprite;
        }
        else if (type == "Fortress")
        {
            spriteRenderer.sprite = gameManager.fortressSprite;
        }
        else if (type == "AcientRuins")
        {
            spriteRenderer.sprite = gameManager.ruinsSprite;
        }
    }
    public void PlaceRandomlyWithinBounds(Vector3 boundsStart,Vector3 boundsEnd)
    {
        gameObject.transform.localPosition =
            new Vector3(
                UnityEngine.Random.Range(boundsStart.x,boundsEnd.x),
                UnityEngine.Random.Range(boundsStart.y, boundsEnd.y),
                0f
            );
    }
    public void SetPosition(Vector3 position)
    {
        gameObject.transform.localPosition = position;
    }
}