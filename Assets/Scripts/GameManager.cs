using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static int worldMapSize = 30;

    static int villageChance = 30;
    static int cityChance = 20;
    static int fortressChance = 5;
    static int acientRuinsChance = 5;

    static int maxConnectionCount = 3;
    static int minConnectionCount = 2;
    static int maxConnectAttempts = 10;
    static float maxLocationConnectionDistance = 10f;

    static float minDistanceBetweenLocations = 5f;

    static float worldMapLocationPullForce = 0.5f;
    static float smoothingCount = 2;

    static float minSmoothingDistance = 5f;

    static int defaultBattleChance = 40;
    static int defaultEventChance = 40;

    static int villageBattleChance = 40;
    static int villageEventChance = 40;

    static int cityBattleChance = 40;
    static int cityEventChance = 40;

    static int fortressBattleChance = 40;
    static int fortressEventChance = 40;

    static int ruinsBattleChance = 40;
    static int ruinsEventChance = 40;

    static Vector3 worldBoundsStart = new Vector3(-30f,-20f,0f);
    static Vector3 worldBoundsEnd = new Vector3(30f, 20f, 0f);

    static Vector3 enemyCastleLocation = new Vector3(0f, 23f, 0f);
    static Vector3 capitolLocation = new Vector3(0f, -21f, 0f);

    static Vector3 fortress1Location = new Vector3(-3f, 20f, 0f);
    static Vector3 fortress2Location = new Vector3(3f, 20f, 0f);

    List<WorldMapNode> worldMap;
    public WorldMapNode playerPosition;

    public Sprite enemyCastleSprite;
    public Sprite capitolSprite;
    public Sprite citySprite;
    public Sprite villageSprite;
    public Sprite fortressSprite;
    public Sprite ruinsSprite;

    public Sprite road;

    GameObject cardLibraryGO;
    CardLibrary cardLibrary;

    public List<Enemy> enemiesInBattle;
    public PlayerData player;

    public GameObject worldMapObject;
    void StartSiege(string nodeName)
    {

    }
    public void MovePlayer(WorldMapNode node)
    {
        playerPosition = node;
        if (playerPosition.type!="EnemyCastle")
        {
            if (!playerPosition.visited)
            {
                int maxRoll = 0;
                int battleChance = 0;
                int eventChance = 0;
                if (playerPosition.type == "Village")
                {
                    maxRoll = villageBattleChance + villageEventChance;
                    battleChance = villageBattleChance;
                    eventChance = villageEventChance;
                }
                else if (playerPosition.type == "City")
                {
                    maxRoll = cityBattleChance + cityEventChance;
                    battleChance = cityBattleChance;
                    eventChance = cityEventChance;
                }
                else if (playerPosition.type == "AcientRuins")
                {
                    maxRoll = ruinsBattleChance + ruinsEventChance;
                    battleChance = ruinsBattleChance;
                    eventChance = ruinsEventChance;
                }
                else if (playerPosition.type == "Fortress")
                {
                    maxRoll = fortressBattleChance + fortressEventChance;
                    battleChance = fortressBattleChance;
                    eventChance = fortressEventChance;
                }
                else
                {
                    maxRoll = defaultBattleChance + defaultEventChance;
                    battleChance = defaultBattleChance;
                    eventChance = defaultEventChance;
                }
                int roll = UnityEngine.Random.Range(0, maxRoll);
                if (roll < battleChance)
                {
                    BeginCombat();
                }
                else if (roll < battleChance + eventChance && roll >= battleChance)
                {
                    //LaunchEvent();
                }
                else
                {
                    //LaunchEvent();
                }
            }
            else
            {

            }
        }
        
    }
    void LaunchEvent(Event gameEvent)
    {

    }
    void BeginCombat()
    {
        worldMapObject.SetActive(false);
        SceneManager.LoadScene("Game");
    }
    void GenerateWorldMap(int nodeCount)
    {
        worldMap = new List<WorldMapNode>();

        worldMap.Add(new WorldMapNode(this,"EnemyCastle", "EnemyCastle",false));
        worldMap[0].SetPosition(enemyCastleLocation);
        worldMap.Add(new WorldMapNode(this, "Capitol", "Capitol"));
        worldMap[1].SetPosition(capitolLocation);
        int maxChance = villageChance + cityChance + fortressChance + acientRuinsChance;
        for (int i = 1; i < nodeCount; i++)
        {
            int newNodeNumber = worldMap.Count;
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
            worldMap[newNodeNumber].PlaceRandomlyWithinBounds(worldBoundsStart, worldBoundsEnd);
            while (CheckIfIsLocationAtNewPosition(worldMap[newNodeNumber].gameObject.transform.localPosition))
            {
                worldMap[newNodeNumber].PlaceRandomlyWithinBounds(worldBoundsStart, worldBoundsEnd);
            }
        }
        int worldNodeCount = worldMap.Count;

        worldMap.Add(new WorldMapNode(this, "BorderFortress", "Fortress", false));
        worldMap.Add(new WorldMapNode(this, "BorderFortress", "Fortress", false));
        worldMap[0].connections.Add(worldMap[worldNodeCount]);
        worldMap[worldNodeCount].connections.Add(worldMap[0]);
        worldMap[0].connections.Add(worldMap[worldNodeCount+1]);
        worldMap[worldNodeCount+1].connections.Add(worldMap[0]);
        worldMap[worldNodeCount].SetPosition(fortress1Location);
        worldMap[worldNodeCount+1].SetPosition(fortress2Location);
        ConnectNodesToTheirClosestNodes();
        for (int i = 1; i < worldMap.Count; i++)
        {
            ConnectIfNotConnected(worldMap[i]);
        }
        //SmoothLocationPositions();
    }
    void DrawWorldMap()
    {

    }
    bool CheckIfIsLocationAtNewPosition(Vector3 position)
    {
        for (int i = 0; i < worldMap.Count-1; i++)
        {
            if (Vector3.Distance(position,worldMap[i].gameObject.transform.localPosition)<minDistanceBetweenLocations)
            {
                return true;
            }
        }
        return false;
    }
    void ConnectNodesToTheirClosestNodes()
    {
        for (int i = 1; i < worldMap.Count; i++)
        {
            List<Tuple<WorldMapNode, float>> distanceToNode = new List<Tuple<WorldMapNode, float>>();
            for (int j = 1; j < worldMap.Count; j++)
            {
                if (i!=j)
                {
                    distanceToNode.Add(new Tuple<WorldMapNode, float>(
                        worldMap[j],
                        Vector3.Distance(worldMap[i].gameObject.transform.localPosition,
                                        worldMap[j].gameObject.transform.localPosition
                                        )
                        ));
                }
            }
            distanceToNode.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            int k = 0;
            while(distanceToNode[k].Item2 < maxLocationConnectionDistance
                || (distanceToNode[k].Item2 >= maxLocationConnectionDistance && worldMap[i].connections.Count < minConnectionCount))
            {
                if (!worldMap[i].connections.Contains(distanceToNode[k].Item1))
                {
                    worldMap[i].connections.Add(distanceToNode[k].Item1);
                    distanceToNode[k].Item1.connections.Add(worldMap[i]);
                }
                k++;
            }
        }
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
        for (int i = 0; i < smoothingCount; i++)
        {
            List<Vector3> positions = PrepareListOfNewPositions();
            for (int j = 0; j < worldMap.Count; j++)
            {
                if (worldMap[j].affectedBySmoothing)
                {
                    worldMap[j].SetPosition(positions[j]);
                }
            }
            NormalizeNodesToBounds();
        }
        NormalizeNodesToBounds();
    }
    void NormalizeNodesToBounds()
    {
        float minX = worldBoundsEnd.x;
        float minY = worldBoundsEnd.y;
        float maxX = worldBoundsStart.x;
        float maxY = worldBoundsStart.y;
        for (int i = 1; i < worldMap.Count; i++)
        {
            float x = worldMap[i].gameObject.transform.position.x;
            float y = worldMap[i].gameObject.transform.position.y;
            if (x < minX) minX = x;
            if (y < minY) minY = y;
            if (x > maxX) maxX = x;
            if (y > maxY) maxY = y;
        }
        for (int i = 1; i < worldMap.Count; i++)
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
                weightSum = weightSum + distance;
            }
            else
            {
                averageX = averageX - item.gameObject.transform.localPosition.x * 5f;
                averageY = averageY - item.gameObject.transform.localPosition.y * 5f;
                weightSum = weightSum + 5f;
            }
        }
        return new Vector3(averageX / weightSum, averageY / weightSum, 0f);
    }
    void ConnectIfNotConnected(WorldMapNode node)
    {
        if (node.type == "EnemyCastle")
        {
            return;
        }
        List<WorldMapNode> toBeVisited = new List<WorldMapNode>();
        List<WorldMapNode> visitedNodes = new List<WorldMapNode>();
        List<Tuple<WorldMapNode, float>> visitedNodesDistance = new List<Tuple<WorldMapNode, float>>();
        visitedNodes.Add(node);
        foreach (WorldMapNode item in node.connections)
        {
            toBeVisited.Add(item);
        }
        while (toBeVisited.Count>0)
        {
            if (toBeVisited[0].type!="EnemyCastle")
            {
                foreach (WorldMapNode item in toBeVisited[0].connections)
                {
                    if (!visitedNodes.Contains(item))
                    {
                        toBeVisited.Add(item);
                    }
                }
                visitedNodes.Add(toBeVisited[0]);
                visitedNodesDistance.Add(new Tuple<WorldMapNode, float>(toBeVisited[0], Vector3.Distance(toBeVisited[0].gameObject.transform.localPosition, worldMap[0].gameObject.transform.localPosition)));
                toBeVisited.RemoveAt(0);
            }
            else
            {
                return;
            }
        }
        visitedNodesDistance.Sort((x, y) => x.Item2.CompareTo(y.Item2));
        WorldMapNode closestUnconnectedNode = null;
        float minDistance = 0;
        foreach (WorldMapNode item in worldMap)
        {
            if (!visitedNodes.Contains(item))
            {
                if (closestUnconnectedNode==null)
                {
                    closestUnconnectedNode = item;
                    minDistance = Vector3.Distance(item.gameObject.transform.localPosition, visitedNodesDistance[0].Item1.gameObject.transform.localPosition);
                }
                else
                {
                    float distance = Vector3.Distance(item.gameObject.transform.localPosition, visitedNodesDistance[0].Item1.gameObject.transform.localPosition);
                    if (distance < minDistance && item.type != "EnemyCastle")
                    {
                        closestUnconnectedNode = item;
                        minDistance = distance;
                    }
                }
            }
        }
        visitedNodesDistance[0].Item1.connections.Add(closestUnconnectedNode);
        closestUnconnectedNode.connections.Add(visitedNodesDistance[0].Item1);
        return;
    }
    private void Start()
    {
        //https://answers.unity.com/questions/1736611/onmouse-events-eg-onmouseenter-not-working-with-ne.html
        PhysicsRaycaster physicsRaycaster = GameObject.FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }

        if (cardLibraryGO==null)
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
        if (worldMap == null)
        {
            worldMapObject = new GameObject("WorldMap");
            worldMapObject.transform.parent = this.gameObject.transform;
            worldMapObject.transform.localPosition = Vector3.zero;
            GenerateWorldMap(worldMapSize);
        }
        playerPosition = worldMap[1];
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

    public bool affectedBySmoothing = true;
    public bool visited = false;

    public WorldMapNode(GameManager _gameManager,string _name,string _type,bool smoothing = true)
    {
        gameManager = _gameManager;
        name = _name;
        type = _type;
        connections = new List<WorldMapNode>();
        state = "normal";
        siegeTime = 0;
        affectedBySmoothing = smoothing;

        gameObject = new GameObject(name);
        gameObject.transform.parent = gameManager.worldMapObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        WorldMapNodeHandle handle = gameObject.AddComponent<WorldMapNodeHandle>();
        handle.SetWorldMapNodeHandle(this,gameManager);

        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = gameObject.transform;
        spriteObject.transform.localPosition = Vector3.zero;

        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

        SetSprite();

        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit,
            spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit,0.2f);
        

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
    public void PlaceRandomlyFortress(Vector3 location)
    {
        gameObject.transform.localPosition =
            new Vector3(
                location.x + UnityEngine.Random.Range(-7f, 7f),
                location.y + -3,
                0f
            );
    }
    public void SetPosition(Vector3 position)
    {
        gameObject.transform.localPosition = new Vector3(position.x,position.y,-5f);
    }

}
public class WorldMapNodeHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerDownHandler, IPointerUpHandler
{
    public WorldMapNode worldMapNode;
    GameManager gameManager;
    public void SetWorldMapNodeHandle(WorldMapNode _worldMapNode,GameManager _gameManager)
    {
        worldMapNode = _worldMapNode;
        gameManager = _gameManager;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameManager.playerPosition.connections.Contains(worldMapNode))
        {
            worldMapNode.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameManager.playerPosition.connections.Contains(worldMapNode))
        {
            worldMapNode.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (gameManager.playerPosition.connections.Contains(worldMapNode))
        {
            gameManager.MovePlayer(worldMapNode);
            worldMapNode.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}