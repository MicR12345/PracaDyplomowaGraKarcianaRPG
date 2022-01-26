using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static int worldNodeCount = 30;

    static int villageChance = 30;
    static int cityChance = 20;
    static int fortressChance = 5;
    static int acientRuinsChance = 5;

    static int minConnectionCount = 2;
    static float maxLocationConnectionDistance = 10f;

    static float minDistanceBetweenLocations = 5f;

    static Vector3 worldBoundsStart = new Vector3(-30f,-23f,0f);
    static Vector3 worldBoundsEnd = new Vector3(30f, 15f, 0f);

    static Vector3 enemyCastleLocation = new Vector3(0f, 20f, 0f);
    static Vector3 capitolLocation = new Vector3(0f, -21f, 0f);

    static Vector3 fortress1Location = new Vector3(-3f, 15f, 0f);
    static Vector3 fortress2Location = new Vector3(3f, 15f, 0f);

    List<WorldMapNode> worldMap;

    static int worldDecoratorMultiplier = 1;
    int[,] worldDecoratorArray;
    static int decoratorArraySmoothingCount = 4;

    public WorldMapNode playerPosition;

    public Sprite enemyCastleSprite;
    public Sprite capitolSprite;
    public Sprite citySprite;
    public Sprite villageSprite;
    public Sprite fortressSprite;
    public Sprite ruinsSprite;

    public Material roadMaterial;

    public Sprite waterSimpleDecoSprite;
    public Sprite mountainDecoSprite;
    public Sprite treeDecoSprite;
    public Sprite plainDecoSprite;

    public List<Sprite> mapDecoBorders;

    public Sprite worldMapSprite;

    public Sprite siegedIconSprite;
    public Sprite defeatedIconSprite;
    public Sprite visitedCheckmarkSprite;
    public Sprite playerLocationSprite;
    GameObject cardLibraryGO;
    CardLibrary cardLibrary;
    GameObject enemyLibraryGO;
    EnemyLibrary enemyLibrary;
    EventLibrary eventLibrary;
    GameObject eventLibraryGO;
    public bool bossFight = false;
    public List<Enemy> enemiesInBattle;
    Event currentEvent;

    public PlayerData player;
    static public Vector3 playerMarkerOffSet = new Vector3(0f,1.5f,-1f);
    GameObject playerMarker;

    List<WorldMapNode> siegedLocations;

    public GameObject worldMapObject;
    public GameObject miscObject;
    [HideInInspector]
    public Vector3 currentConnectedExpansionScale = new Vector3(1f, 1f, 1f);
    static Vector3 maxSize = new Vector3(1.2f, 1.2f, 1f);
    static Vector3 minSize = new Vector3(1f, 1f, 1f);
    bool expanding = true;
    float expansionSpeed = 0.06f;
    void StartSiege(WorldMapNode node)
    {
        if (!siegedLocations.Contains(node))
        {
            siegedLocations.Add(node);
            node.StartSiege();
        }
    }
    public void MovePlayer(WorldMapNode node)
    {
        playerPosition.SetAsVisited();
        playerPosition = node;
        MovePlayerMark();
        if (playerPosition.type!="EnemyCastle")
        {
            if (!playerPosition.visited && playerPosition.siegeTime>0 && !siegedLocations.Contains(playerPosition))
            {
                    RollEvent();
            }
            if (!playerPosition.visited && playerPosition.siegeTime > 0 && siegedLocations.Contains(playerPosition))
            {
                RollEventSiege();
            }
            else if (!playerPosition.visited && playerPosition.siegeTime <= 0)
            {
                RollEventDefeated();
                
            }
        }
        else
        {
            currentEvent = eventLibrary.FindFinalBossEvent();
        }
        LaunchEvent();
        ProgressSieges();
        playerPosition.SetAsVisited();
        Debug.Log(player.deck.Count);
        SaveManager.SaveGame(worldMap, worldDecoratorArray, siegedLocations, player, playerPosition);
    }
    void RollEvent()
    {
        List<Event> acceptableEvents = new List<Event>();
        foreach (Event i in eventLibrary.eventList)
        {
            if (i.type == playerPosition.type || i.type == "Any")
            {
                acceptableEvents.Add(i);
            }
        }
        int rollEvent = UnityEngine.Random.Range(0, acceptableEvents.Count);

        currentEvent = acceptableEvents[rollEvent];
    }
    void RollEventSiege()
    {
        List<Event> acceptableEvents = new List<Event>();
        foreach (Event i in eventLibrary.eventList)
        {
            if (i.type == "Siege")
            {
                acceptableEvents.Add(i);
            }
        }
        int rollEvent = UnityEngine.Random.Range(0, acceptableEvents.Count);

        currentEvent = acceptableEvents[rollEvent];
    }
    void RollEventDefeated()
    {
        List<Event> acceptableEvents = new List<Event>();
        foreach (Event i in eventLibrary.eventList)
        {
            if (i.type == "Defeated")
            {
                acceptableEvents.Add(i);
            }
        }
        int rollEvent = UnityEngine.Random.Range(0, acceptableEvents.Count);

        currentEvent = acceptableEvents[rollEvent];
    }
    void LaunchEvent()
    {
        GameObject eventPopupGO = GameObject.Find("EventPopup");
        if (eventPopupGO == null)
        {
            Debug.LogError("Event popup object not found");
        }
        EventPopupHandle eventPopupHandle = eventPopupGO.GetComponent<EventPopupHandle>();
        eventPopupHandle.eventPopupCanvas.SetActive(true);
        eventPopupHandle.eventBackgroundImage.sprite = currentEvent.eventBackground;
        eventPopupHandle.descriptionText.text = currentEvent.description;
        if (currentEvent.choices.Count>=1)
        {
            eventPopupHandle.button1.SetActive(true);
            eventPopupHandle.button1Text.text = currentEvent.choices[0].text; 
        }
        if (currentEvent.choices.Count >= 2)
        {
            eventPopupHandle.button2.SetActive(true);
            eventPopupHandle.button2Text.text = currentEvent.choices[1].text;
        }
        else
        {
            eventPopupHandle.button2.SetActive(false);
        }
        if (currentEvent.choices.Count >= 3)
        {
            eventPopupHandle.button3.SetActive(true);
            eventPopupHandle.button3Text.text = currentEvent.choices[2].text;
        }
        else
        {
            eventPopupHandle.button3.SetActive(false);
        }
        if (currentEvent.choices.Count >= 4)
        {
            eventPopupHandle.button4.SetActive(true);
            eventPopupHandle.button4Text.text = currentEvent.choices[3].text;
        }
        else
        {
            eventPopupHandle.button4.SetActive(false);
        }
    }
    public void HandleEvent(int choice)
    {
        ChoiceOption choiceOption = currentEvent.choices[choice];
        if (choiceOption.addCards!=null && choiceOption.addCards.Count>0)
        {
            foreach (string item in choiceOption.addCards)
            {
                player.deck.Add(new DeckCard(cardLibrary.FindCardByName(item)));
            }
        }
        Tag BattleRandom = choiceOption.FindTag("BattleRandom");
        if (BattleRandom != null)
        {
            enemiesInBattle = new List<Enemy>();
            int enemyCount = UnityEngine.Random.Range(1, Mathf.FloorToInt(BattleRandom.value+1));
            for (int i = 0; i < enemyCount; i++)
            {
                enemiesInBattle.Add(enemyLibrary.PickRandomEnemy());
            }
            BeginCombat();
        }
        Tag FinalBoss = choiceOption.FindTag("FinalBoss");
        if (FinalBoss != null)
        {
            enemiesInBattle = new List<Enemy>();
            bossFight = true;
            enemiesInBattle.Add(enemyLibrary.PickEnemyByName("FinalBoss"));
            BeginCombat();
        }
        Tag RandomCardR0 = choiceOption.FindTag("RandomCardR0");
        if (RandomCardR0 != null)
        {
            for (int i = 0; i < RandomCardR0.value; i++)
            {
                player.AddToPlayerDeck(new DeckCard(cardLibrary.FindRandomCardByRarity(0)));
            } 
        }
        Tag RandomCardR1 = choiceOption.FindTag("RandomCardR1");
        if (RandomCardR1!=null)
        {
            for (int i = 0; i < RandomCardR1.value; i++)
            {
                player.AddToPlayerDeck(new DeckCard(cardLibrary.FindRandomCardByRarity(1)));
            }
        }
        Tag RandomCardR2 = choiceOption.FindTag("RandomCardR2");
        if (RandomCardR2 != null)
        {
            for (int i = 0; i < RandomCardR2.value; i++)
            {
                player.AddToPlayerDeck(new DeckCard(cardLibrary.FindRandomCardByRarity(2)));
            }
        }
        Tag RandomCardR3 = choiceOption.FindTag("RandomCardR3");
        if (RandomCardR3 != null)
        {
            for (int i = 0; i < RandomCardR3.value; i++)
            {
                player.AddToPlayerDeck(new DeckCard(cardLibrary.FindRandomCardByRarity(3)));
            }
        }
        Tag IncreaseMaxHp = choiceOption.FindTag("IncreaseMaxHp");
        if (IncreaseMaxHp != null)
        {
            player.healthMax = player.healthMax + IncreaseMaxHp.value;
        }
        Tag IncreaseMaxAp = choiceOption.FindTag("IncreaseMaxAp");
        if (IncreaseMaxAp != null)
        {
            player.actionPointsMax = player.actionPointsMax + Mathf.FloorToInt(IncreaseMaxAp.value);
        }

    }
    void BeginCombat()
    {
        worldMapObject.SetActive(false);
        SceneManager.LoadScene("Battle");
    }
    void CreatePlayerMark()
    {
        playerMarker = new GameObject("Player Marker");
        playerMarker.transform.parent = worldMapObject.transform;
        playerMarker.transform.position = playerPosition.gameObject.transform.position + playerMarkerOffSet;
        SpriteRenderer spriteRenderer = playerMarker.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = playerLocationSprite;
    }
    void MovePlayerMark()
    {
        playerMarker.transform.position = playerPosition.gameObject.transform.position + playerMarkerOffSet;
    }
    void GenerateWorldMap()
    {
        worldMap = new List<WorldMapNode>();
        CreateMainLocations();
        CreateBorderFortresses();
        CreateWorldLocationsRandom();
        
        ConnectNodesToTheirClosestNodes();
        for (int i = 1; i < worldMap.Count; i++)
        {
            ConnectIfNotConnected(worldMap[i]);
        }
        CreateWorldDecorationsArray();
        for (int i = 0; i < decoratorArraySmoothingCount; i++)
        {
            SmoothWorldDecorationArray();
        }
        DrawDecorators();
    }
    void CreateWorldLocationsRandom()
    {
        int maxChance = villageChance + cityChance + fortressChance + acientRuinsChance;
        for (int i = 1; i < worldNodeCount; i++)
        {
            int newNodeNumber = worldMap.Count;
            int roll = UnityEngine.Random.Range(0, maxChance);
            if (roll < villageChance)
            {
                worldMap.Add(new WorldMapNode(this, "Village", "Village"));
            }
            else if (roll >= villageChance && roll < villageChance + cityChance)
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
    }
    void CreateMainLocations()
    {
        worldMap.Add(new WorldMapNode(this, "EnemyCastle", "EnemyCastle"));
        worldMap[0].SetPosition(enemyCastleLocation);
        worldMap.Add(new WorldMapNode(this, "Capitol", "Capitol"));
        worldMap[1].SetPosition(capitolLocation);
    }
    void CreateBorderFortresses()
    {
        int worldNodeCount = worldMap.Count;

        worldMap.Add(new WorldMapNode(this, "BorderFortress", "Fortress"));
        worldMap.Add(new WorldMapNode(this, "BorderFortress", "Fortress"));
        worldMap[0].connections.Add(worldMap[worldNodeCount]);
        worldMap[worldNodeCount].connections.Add(worldMap[0]);
        worldMap[0].connections.Add(worldMap[worldNodeCount + 1]);
        worldMap[worldNodeCount + 1].connections.Add(worldMap[0]);
        worldMap[worldNodeCount].SetPosition(fortress1Location);
        worldMap[worldNodeCount + 1].SetPosition(fortress2Location);
        DrawRoad(worldMap[0].gameObject.transform.position, worldMap[worldNodeCount].gameObject.transform.position);
        DrawRoad(worldMap[0].gameObject.transform.position, worldMap[worldNodeCount + 1].gameObject.transform.position);
    }
    void CreateWorldDecorationsArray()
    {
        worldDecoratorArray = new int[worldDecoratorMultiplier * Mathf.FloorToInt(Mathf.Abs(worldBoundsStart.x) + worldBoundsEnd.x),
                                      worldDecoratorMultiplier * Mathf.FloorToInt(Mathf.Abs(worldBoundsStart.y) + enemyCastleLocation.y + 1)];
        int sizeX = worldDecoratorArray.GetLength(0);
        int sizeY = worldDecoratorArray.GetLength(1);
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
               worldDecoratorArray[i, j] = UnityEngine.Random.Range(-40, 40);
            }
        }
        foreach (WorldMapNode item in worldMap)
        {
            worldDecoratorArray[Mathf.FloorToInt(worldDecoratorMultiplier * 
                                (item.gameObject.transform.localPosition.x + Mathf.Abs(worldBoundsStart.x))),
                                Mathf.FloorToInt(worldDecoratorMultiplier * 
                                (item.gameObject.transform.localPosition.y + Mathf.Abs(worldBoundsStart.y)))]
                                = 50;
        }
    }
    void SmoothWorldDecorationArray()
    {
        int sizeX = worldDecoratorArray.GetLength(0);
        int sizeY = worldDecoratorArray.GetLength(1);
        int[,] newDecoArray = new int[sizeX, sizeY];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                newDecoArray[i, j] = worldDecoratorArray[i, j];
            }
        }
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (worldDecoratorArray[i,j] == 50)
                {
                    newDecoArray[i, j] = 50;
                }
                else
                {
                    int sum = 0;
                    int count = 0;
                    for (int n = i-1; n < i+1; n++)
                    {
                        if (n < 0) continue;
                        else if (n >= sizeX) continue;
                        for (int m = j-1; m < j+1; m++)
                        {
                            if (m < 0) continue;
                            else if (m >= sizeY) continue;
                            if (worldDecoratorArray[n,m] == 50)
                            {
                                sum = sum + 50;
                                count++;
                            }
                            else
                            {
                                sum = sum + worldDecoratorArray[n, m];
                                count++;
                            }
                        }
                    }
                    newDecoArray[i, j] = Mathf.FloorToInt(sum / count);

                }
            }
        }
        worldDecoratorArray = newDecoArray;
    }
    void DrawDecorators()
    {
        int sizeX = worldDecoratorArray.GetLength(0);
        int sizeY = worldDecoratorArray.GetLength(1);
        for (int i = 1; i < sizeX-1; i++)
        {
            for (int j = 1; j < sizeY-1; j++)
            {
                GameObject deco = new GameObject("mapDecoration");
                deco.transform.parent = worldMapObject.transform;
                deco.transform.localPosition = new Vector3((i / (worldDecoratorMultiplier * 1f)) + worldBoundsStart.x, (j / (worldDecoratorMultiplier * 1f)) + worldBoundsStart.y, 0f);

                SpriteRenderer spriteRenderer = deco.AddComponent<SpriteRenderer>();
                if (worldDecoratorArray[i,j] <0)
                {
                    /*if (worldDecoratorArray[i,j]<-20)
                    {
                        spriteRenderer.sprite = waterWaveDecoSprite;
                        //deco.transform.localPosition += new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
                    }
                    else */spriteRenderer.sprite = waterSimpleDecoSprite;
                }
                else
                {
                    int border = CheckForBorder(i, j);
                    if (border>0)
                    {
                        spriteRenderer.sprite = mapDecoBorders[border-1];
                    }
                    else
                    {
                        if (worldDecoratorArray[i, j] >= 0 && worldDecoratorArray[i, j] < 10)
                        {
                            spriteRenderer.sprite = plainDecoSprite;
                        }
                        else if (worldDecoratorArray[i, j] >= 10 && worldDecoratorArray[i, j] < 20)
                        {
                            spriteRenderer.sprite = treeDecoSprite;
                        }
                        else if (worldDecoratorArray[i, j] >= 20 && worldDecoratorArray[i, j] < 30)
                        {
                            spriteRenderer.sprite = mountainDecoSprite;
                        }
                    }
                }
                spriteRenderer.sortingOrder = -1;
            }
        }
    }
    int CheckForBorder(int x,int y)
    {
        int sizeX = worldDecoratorArray.GetLength(0);
        int sizeY = worldDecoratorArray.GetLength(1);
        int output = 0;
        if (x-1>0 && worldDecoratorArray[x-1,y]<0)
        {
            output += 1;
        }
        if (x + 1 < sizeX && worldDecoratorArray[x + 1, y] < 0)
        {
            output += 2;
        }
        if (y - 1 > 0 && worldDecoratorArray[x, y - 1] < 0)
        {
            output += 8;
        }
        if (y + 1 < sizeY && worldDecoratorArray[x, y + 1] < 0)
        {
            output += 4;
        }
        return output;
    }
    void DrawRoad(Vector3 start,Vector3 end)
    {
        GameObject roadObject = new GameObject("Road");
        roadObject.transform.parent = worldMapObject.transform;
        MeshFilter meshFilter = roadObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = roadObject.AddComponent<MeshRenderer>();
        float distance = Vector3.Distance(start, end);
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(-0.3f, -distance / 2f, -1f));
        verts.Add(new Vector3(0.3f, -distance / 2f, -1f));
        verts.Add(new Vector3(0.3f, distance / 2f, -1f));
        verts.Add(new Vector3(-0.3f, distance / 2f, -1f));
        int[] triangles = { 0, 3, 1, 1, 3, 2 };
        Vector2[] uvs =
        {
            new Vector2(0f,0f),
            new Vector2(1f,0f),
            new Vector2(1f,1f),
            new Vector2(0f,1f)
        };
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles;
        mesh.uv = uvs;

        meshFilter.mesh = mesh;
        meshRenderer.material = roadMaterial;
        roadObject.transform.localPosition = Vector3.zero;
        Vector3 vectorToEnd = start - end;
        float angle = Mathf.Atan2(vectorToEnd.y, vectorToEnd.x) * Mathf.Rad2Deg;
        roadObject.transform.LookAt(end);
        roadObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle+90f));


        roadObject.transform.localPosition = (start + end) / 2f + new Vector3(0f,0f,2f);
        
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
                    DrawRoad(worldMap[i].gameObject.transform.position, distanceToNode[k].Item1.gameObject.transform.position);
                }
                k++;
            }
        }
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
        DrawRoad(visitedNodesDistance[0].Item1.gameObject.transform.position, closestUnconnectedNode.gameObject.transform.position);
        return;
    }
    void ProgressSieges()
    {
        List<WorldMapNode> previouslySiegedLocations = new List<WorldMapNode>(siegedLocations);
        foreach (WorldMapNode item in previouslySiegedLocations)
        {
            if (item.siegeTime>0)
            {
                item.siegeTime--;
                item.UpdateSiegeComponents();
            }
            else
            {
                foreach (WorldMapNode connected in item.connections)
                {
                    StartSiege(connected);
                }
            }
        }
        if (worldMap[1].siegeTime<1)
        {
            LoseGame();
        }
    }
    public void LeaveToMenu()
    {
        GameObject.Destroy(miscObject);
        GameObject.Destroy(worldMapObject);
        SceneManager.LoadScene("MainMenu");
        GameObject.Destroy(this.gameObject);
    }
    public void WinGame()
    {
        GameObject.Destroy(miscObject);
        GameObject.Destroy(worldMapObject);
        SaveManager.RemoveSavedGame();
        SceneManager.LoadScene("VictoryScene");
        GameObject.Destroy(this.gameObject);
    }
    public void LoseGame()
    {
        GameObject.Destroy(miscObject);
        GameObject.Destroy(worldMapObject);
        SaveManager.RemoveSavedGame();
        SceneManager.LoadScene("LoseGame");
        GameObject.Destroy(this.gameObject);
    }
    private void Start()
    {
        bool loadingSave = false;
        if (SceneManager.GetActiveScene().name == "SaveLoadScreen") loadingSave = true;
        //https://answers.unity.com/questions/1736611/onmouse-events-eg-onmouseenter-not-working-with-ne.html
        /*PhysicsRaycaster physicsRaycaster = GameObject.FindObjectOfType<PhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<PhysicsRaycaster>();
        }*/

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
        if (enemyLibraryGO == null)
        {
            enemyLibraryGO = GameObject.Find("ENEMY LIBRARY");
            if (enemyLibraryGO == null)
            {
                Debug.LogError("Enemy library object not found");
            }
            else
            {
                if(enemyLibrary == null)
                {
                    enemyLibrary = enemyLibraryGO.GetComponent<EnemyLibrary>();
                }
                enemyLibrary.LoadAllEnemies();
                Debug.Log("Enemy library found with " + enemyLibrary.enemyList.Count + "enemies");
            }
        }
        if(eventLibraryGO == null)
        {
            eventLibraryGO = GameObject.Find("EVENT LIBRARY");
            if(eventLibraryGO == null)
            {
                Debug.LogError("Event library object not found");
            }
            else
            {
                if(eventLibrary == null)
                {
                    eventLibrary = eventLibraryGO.GetComponent<EventLibrary>();
                }
                eventLibrary.LoadAllEvents();
                Debug.Log("Event library found with " + eventLibrary.eventList.Count + "events");
            }
        }
        if (!loadingSave)
        {
            if (worldMap == null)
            {
                worldMapObject = new GameObject("WorldMap");
                worldMapObject.transform.parent = this.gameObject.transform;
                worldMapObject.transform.localPosition = Vector3.zero;

                SpriteRenderer spriteRenderer = worldMapObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = worldMapSprite;
                spriteRenderer.sortingOrder = -50;

                GenerateWorldMap();

            }
            player = new PlayerData();
            playerPosition = worldMap[1];

            siegedLocations = new List<WorldMapNode>();
            StartSiege(worldMap[0]);
        }
        else
        {
            if (worldMap == null)
            {
                worldMapObject = new GameObject("WorldMap");
                worldMapObject.transform.parent = this.gameObject.transform;
                worldMapObject.transform.localPosition = Vector3.zero;

                SpriteRenderer spriteRenderer = worldMapObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = worldMapSprite;
                spriteRenderer.sortingOrder = -50;

                worldMap = new List<WorldMapNode>();
            }
            SaveManager.SaveGameData saveGameData = SaveManager.LoadGame();
            foreach (SaveManager.WorldMapNodeData item in saveGameData.worldMapData.worldMapNodes)
            {
                worldMap.Add(new WorldMapNode(this, item));
            }
            foreach (WorldMapNode item in worldMap)
            {
                foreach (int connectionIndex in item.loadedConnections)
                {
                    item.connections.Add(worldMap[connectionIndex]);
                    worldMap[connectionIndex].connections.Add(item);
                    worldMap[connectionIndex].loadedConnections.Remove(worldMap.IndexOf(item));
                    DrawRoad(item.gameObject.transform.position, worldMap[connectionIndex].gameObject.transform.position);
                }
            }
            int sizeY = saveGameData.decoratorArray.decoratorArrayLines.Count;
            int sizeX = saveGameData.decoratorArray.decoratorArrayLines[0].data.Count;
            worldDecoratorArray = new int[sizeX, sizeY];
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    worldDecoratorArray[x,y] = saveGameData.decoratorArray.decoratorArrayLines[y].data[x];
                }
            }
            DrawDecorators();
            siegedLocations = new List<WorldMapNode>();
            foreach (int item in saveGameData.siegedLocations)
            {
                siegedLocations.Add(worldMap[item]);
            }
            foreach (WorldMapNode item in siegedLocations)
            {
                item.StartSiege();
            }
            player = new PlayerData(saveGameData.playerSaveData, cardLibrary);
            playerPosition = worldMap[saveGameData.playerSaveData.playerLocation];
        }
        CreatePlayerMark();
        //https://forum.unity.com/threads/stop-a-function-till-scene-is-loaded.546646/
        if (!loadingSave)
        {
            currentEvent = eventLibrary.FindStartEvent();
            newScene("World");
        }
        else
        {
            SceneManager.LoadScene("World");
        }
    }
    private void Update()
    {
        if (currentConnectedExpansionScale == minSize && !expanding)
        {
            expanding = true;
            currentConnectedExpansionScale = Vector3.Lerp(currentConnectedExpansionScale, maxSize, expansionSpeed);
        }
        else if (currentConnectedExpansionScale == maxSize && expanding)
        {
            expanding = false;
            currentConnectedExpansionScale = Vector3.Lerp(currentConnectedExpansionScale, minSize, expansionSpeed);
        }
        if (expanding)
        {
            currentConnectedExpansionScale = Vector3.Lerp(currentConnectedExpansionScale, maxSize, expansionSpeed);
        }
        else
        {
            currentConnectedExpansionScale = Vector3.Lerp(currentConnectedExpansionScale, minSize, expansionSpeed);
        }
    }
    public void newScene(string scene)
    {
        SceneManager.LoadScene(scene);

        if (SceneManager.GetActiveScene().name != scene)
        {
            StartCoroutine(waitForSceneLoad(scene));
        }
    }

    IEnumerator waitForSceneLoad(string scene)
    {
        while (SceneManager.GetActiveScene().name != scene)
        {
            yield return null;
        }

        // Do anything after proper scene has been loaded
        if (SceneManager.GetActiveScene().name == scene)
        {
            LaunchEvent();
        }
    }

}
public class WorldMapNode
{
    GameManager gameManager;

    public string name;
    public string type;
    public List<WorldMapNode> connections;

    public GameObject gameObject;

    GameObject spriteObject;
    SpriteRenderer spriteRenderer;
    GameObject siegeIconObject;
    SpriteRenderer siegeIconRenderer;
    GameObject visitedIconObject;
    SpriteRenderer visitedIconRenderer;

    public int siegeTime;

    public bool visited = false;

    public List<int> loadedConnections;

    Vector3 checkMarkOffset;
    Vector3 siegeMarkOffset;
    public WorldMapNode(GameManager _gameManager,string _name,string _type)
    {
        gameManager = _gameManager;
        name = _name;
        type = _type;
        connections = new List<WorldMapNode>();
        SetSiegeTime();

        gameObject = new GameObject(name);
        gameObject.transform.parent = gameManager.worldMapObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        WorldMapNodeHandle handle = gameObject.AddComponent<WorldMapNodeHandle>();
        handle.SetWorldMapNodeHandle(this,gameManager);

        CreateSpriteObject();

        SetSprite();

        CreateCollider();

        CreateSiegeIcon();
 
        CreateVisitedIcon();
    }
    public WorldMapNode(GameManager _gameManager,SaveManager.WorldMapNodeData worldMapNodeData)
    {
        gameManager = _gameManager;
        name = worldMapNodeData.name;
        type = worldMapNodeData.type;
        siegeTime = worldMapNodeData.siegeTime;
        connections = new List<WorldMapNode>();
        loadedConnections = new List<int>(worldMapNodeData.connections);
        visited = worldMapNodeData.visited;
        gameObject = new GameObject(name);
        gameObject.transform.parent = gameManager.worldMapObject.transform;
        gameObject.transform.localPosition = new Vector3(worldMapNodeData.x,worldMapNodeData.y,-5f);
        WorldMapNodeHandle handle = gameObject.AddComponent<WorldMapNodeHandle>();
        handle.SetWorldMapNodeHandle(this, gameManager);

        CreateSpriteObject();

        SetSprite();

        CreateCollider();

        CreateSiegeIcon();
        CreateVisitedIcon();
    }
    void CreateCollider()
    {
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit,
            spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit, 0.2f);
    }
    void CreateSpriteObject()
    {
        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = gameObject.transform;
        spriteObject.transform.localPosition = Vector3.zero + new Vector3(0f, 0.6f);
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
    }
    void CreateVisitedIcon()
    {
        visitedIconObject = new GameObject("VisitedIcon");
        visitedIconObject.transform.parent = gameObject.transform;
        visitedIconObject.transform.localPosition = checkMarkOffset;
        visitedIconRenderer = visitedIconObject.AddComponent<SpriteRenderer>();
        visitedIconRenderer.sprite = gameManager.visitedCheckmarkSprite;
        if (!visited)
        {
            visitedIconObject.SetActive(false);
        }
    }
    void SetSiegeTime()
    {
        if (type == "EnemyCastle")
        {
            siegeTime = 0;
        }
        else if (type == "Capitol")
        {
            siegeTime = 5;
        }
        else if (type == "City")
        {
            siegeTime = 3;
        }
        else if (type == "Village")
        {
            siegeTime = 1;
        }
        else if (type == "Fortress")
        {
            siegeTime = 5;
        }
        else if (type == "AcientRuins")
        {
            siegeTime = 1;
        }
        else
        {
            siegeTime = 0;
        }
    }
    void CreateSiegeIcon()
    {
        siegeIconObject = new GameObject("SiegeIcon");
        siegeIconObject.transform.parent = gameObject.transform;
        siegeIconObject.transform.localPosition = siegeMarkOffset;
        siegeIconRenderer = siegeIconObject.AddComponent<SpriteRenderer>();
        if (type == "EnemyCastle")
        {
            siegeIconObject.SetActive(false);
        }
    }

    void SetSprite()
    {
        if (type == "EnemyCastle")
        {
            spriteRenderer.sprite = gameManager.enemyCastleSprite;
            checkMarkOffset = new Vector3(3f, -1f, 0f);
            siegeMarkOffset = new Vector3(3f,1f,-0.5f);
        }
        else if (type == "Capitol")
        {
            spriteRenderer.sprite = gameManager.capitolSprite;
            checkMarkOffset = new Vector3(2f, -1f, 0f);
            siegeMarkOffset = new Vector3(2f, 3f, -0.5f);
        }
        else if (type == "City")
        {
            spriteRenderer.sprite = gameManager.citySprite;
            checkMarkOffset = new Vector3(1f, -0.75f, 0f);
            siegeMarkOffset = new Vector3(1f, 2.25f, -0.5f);
        }
        else if (type == "Village")
        {
            spriteRenderer.sprite = gameManager.villageSprite;
            checkMarkOffset = new Vector3(1.5f, -1f, 0f);
            siegeMarkOffset = new Vector3(1.15f, 2.7f, -0.5f);
        }
        else if (type == "Fortress")
        {
            spriteRenderer.sprite = gameManager.fortressSprite;
            checkMarkOffset = new Vector3(1f, -1f, 0f);
            siegeMarkOffset = new Vector3(1f, 2f, -0.5f);
        }
        else if (type == "AcientRuins")
        {
            spriteRenderer.sprite = gameManager.ruinsSprite;
            checkMarkOffset = new Vector3(1.5f, -1f, 0f);
            siegeMarkOffset = new Vector3(1.5f, 2f, -0.5f);
        }
    }
    public void PlaceRandomlyWithinBounds(Vector3 boundsStart,Vector3 boundsEnd)
    {
        gameObject.transform.localPosition =
            new Vector3(
                UnityEngine.Random.Range(boundsStart.x+3f,boundsEnd.x-3f),
                UnityEngine.Random.Range(boundsStart.y+3f, boundsEnd.y-3f),
                -5f
            );
    }
    public void PlaceRandomlyFortress(Vector3 location)
    {
        gameObject.transform.localPosition =
            new Vector3(
                location.x + UnityEngine.Random.Range(-7f, 7f),
                location.y + -3,
                -5f
            );
    }
    public void SetPosition(Vector3 position)
    {
        gameObject.transform.localPosition = new Vector3(position.x,position.y,-5f);
    }
    public void StartSiege()
    {
        siegeIconRenderer.sprite = gameManager.siegedIconSprite;
        UpdateSiegeComponents();
    }
    public void UpdateSiegeComponents()
    {
        if (siegeTime<1)
        {
            siegeIconRenderer.sprite = gameManager.defeatedIconSprite;
        }
    }
    public void SetAsVisited()
    {
        visited = true;
        visitedIconObject.SetActive(true);
    }
}
public class WorldMapNodeHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerDownHandler, IPointerUpHandler
{
    public WorldMapNode worldMapNode;
    GameManager gameManager;

    bool affectedByPointer = false;
    public void SetWorldMapNodeHandle(WorldMapNode _worldMapNode,GameManager _gameManager)
    {
        worldMapNode = _worldMapNode;
        gameManager = _gameManager;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        affectedByPointer = true;
        if (gameManager.playerPosition.connections.Contains(worldMapNode))
        {
            worldMapNode.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        affectedByPointer = false;
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
    void Update()
    {
        if (gameManager.playerPosition.connections.Contains(worldMapNode) && !affectedByPointer)
        {
            worldMapNode.gameObject.transform.localScale = gameManager.currentConnectedExpansionScale;
        }
        else if(!affectedByPointer)
        {

            worldMapNode.gameObject.transform.localScale = Vector3.one;
        }
    }
}