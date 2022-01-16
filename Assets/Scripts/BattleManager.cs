using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public GameObject Background;
    public GameObject GameLight;
    public Camera GameCamera;
    public PointerControl pointerControl;
    public List<float> enenmyTimers;
    //shared card library
    [HideInInspector]
    public CardLibrary cardLibrary;
    public EnemyLibrary enemyLibrary;

    GameObject playerObject;
    [HideInInspector]
    public Player player;

    public Sprite debugPlayerSprite;

    public Vector2 playerPosition;

    List<Enemy> enemies;
    public List<Vector2> enemyPositions;
    
    [HideInInspector]
    public List<Tuple<int,int>> battleQueue;
    public bool inBattle;

    GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("World").GetComponent<GameManager>();
        cardLibrary = GameObject.Find("CARD LIBRARY").GetComponent<CardLibrary>();
        enemyLibrary = GameObject.Find("ENEMY LIBRARY").GetComponent<EnemyLibrary>();
        enemies = enemyLibrary.enemyList;
        
        OnBattleStart();
        gameManager.worldMapObject.SetActive(true);
        //SceneManager.LoadScene("World");
    }
    void OnBattleStart()
    {
        inBattle = true;
        if (player==null)
        {
            CreatePlayerObject();
        }
        enemies = new List<Enemy>();
        //CheatSpawnDebugEnemies(4);
        enemies = gameManager.enemiesInBattle;
        foreach(Enemy enemy in enemies)
        {
            enemy.CreateFightingEnemy(this);
        }
        PlaceEnemies();
        //CreateBattleQueue();

        CheatGiveDebugCardsToDeck(10);
        player.PrepareHandBeforeBattle();
    }
/*    void CreateBattleQueue()
    {
        battleQueue = new List<Tuple<int, int>>();

        battleQueue.Add(new Tuple<int, int>(-1, player.initiative));
        for (int i = 0; i < enemies.Count && i< enemyPositions.Count; i++)
        {
            bool added = false;
            for (int j = 0; j < battleQueue.Count; j++)
            {
                if (enemies[i].initiative>battleQueue[j].Item2)
                {
                    battleQueue.Insert(j, new Tuple<int, int>(i, enemies[i].initiative));
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                battleQueue.Add(new Tuple<int, int>(i, enemies[i].initiative));
            }
        }
    }*/
    /*public bool IsPlayerTurn()
    {
        if (battleQueue!=null && battleQueue[0]!=null)
        {
            if (battleQueue[0].Item1 == -1)
            {
                return true;
            }
            else return false;
        }
        else
        {
            Debug.LogError("Tried to check if it's player turn while there is no battle queue");
            return false;
        }
    }*/
    void CreatePlayerObject()
    {
        playerObject = new GameObject();
        playerObject.transform.parent = this.gameObject.transform;
        playerObject.transform.localPosition = Vector3.zero;
        player = playerObject.AddComponent<Player>();
        player.PlayerObjectSetup(this,gameManager, debugPlayerSprite,playerPosition);
    }

    void PlaceEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (i<enemyPositions.Count)
            {
                enemies[i].SetPosition(enemyPositions[i]);
            }
            else
            {
                Debug.LogError("More enemies than aviable slots");
            }
        }
    }
    public void CardWasMovedOntoEnemy(DeckCard card,Enemy enemy)
    {
       // if (IsPlayerTurn())
       // {
       //     if (!enemy.isDead)
       //     {
                Debug.Log(enemy.health);
                ApplyCard(card, enemy);
                player.CheckForCardRemoval();
                PlayerMadeMove();
       //     }
       //     else
       //    {
       //         player.SetupCardLocation();
       //    }
       // }
    }
    void PlayerMadeMove()
    {
        Tuple<int, int> player;
        player = battleQueue[0];
        battleQueue.RemoveAt(0);
        battleQueue.Add(player);
    }
    void ApplyCard(DeckCard card, Enemy enemy)
    {
        foreach (Effect item in card.card.effects)
        {
            enemy.ApplyCardEffect(item);
        }
        Tag destroyTag = card.card.FindCardTag("destroy");
        Tag exhaustTag = card.card.FindCardTag("exhaust");
        card.discarded = true;
        if (destroyTag != null)
        {
            card.destroyed = true;
        }
        if (exhaustTag != null)
        {
            card.exhausted = Mathf.FloorToInt(exhaustTag.value);
        }
    }
    void ApplyCardToPlayer(DeckCard card, Player player)
    {
        foreach (Effect item in card.card.effects)
        {
            player.ApplyCardEffect(item);
        }
        Tag destroyTag = card.card.FindCardTag("destroy");
        Tag exhaustTag = card.card.FindCardTag("exhaust");
        card.discarded = true;
        if (destroyTag != null)
        {
            card.destroyed = true;
        }
        if (exhaustTag != null)
        {
            card.exhausted = Mathf.FloorToInt(exhaustTag.value);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (inBattle)
        {
           // if (battleQueue[0].Item1 != -1)
           // {
           //    enemies[0].MakeAMove();
                
                
           // }
        }
    }

    //DEBUGGING FUNCTIONS

    void CheatGiveDebugCardsToDeck(int count)
    {
        for (int i = 0; i < 10; i++)
        {
            player.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
        }
    }
    /*void CheatSpawnDebugEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            enemies.Add(new Enemy(this, "debugEnemy", 10, 10, 10, 1, debugPlayerSprite));
        }
    }*/
}
