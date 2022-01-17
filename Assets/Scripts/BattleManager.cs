using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    static float playerCardGiveTime = 5f;
    float playerCardGiveTimer = playerCardGiveTime;

    static float playerResourceGiveTime = 3f;
    float playerResourceGiveTimer = playerResourceGiveTime;

    static float effectsTime = 10f;
    float effectsTimer = effectsTime;

    List<float> enemyTimers;

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
    public bool inBattle;

    GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("World").GetComponent<GameManager>();
        cardLibrary = GameObject.Find("CARD LIBRARY").GetComponent<CardLibrary>();
        enemyLibrary = GameObject.Find("ENEMY LIBRARY").GetComponent<EnemyLibrary>();
        enemies = enemyLibrary.enemyList;
        
        OnBattleStart();
        //gameManager.worldMapObject.SetActive(true);
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
        enemyTimers = new List<float>();
        foreach (Enemy enemy in enemies)
        {
            enemy.CreateFightingEnemy(this);
            enemyTimers.Add(enemy.spellDuration);
        }
        PlaceEnemies();

        player.actionPoints = player.data.actionPointsMax;

        CheatGiveDebugCardsToDeck(10);
        player.PrepareHandBeforeBattle();
        player.DealAFullHand();
    }
    void CreatePlayerObject()
    {
        playerObject = new GameObject();
        playerObject.transform.parent = this.gameObject.transform;
        playerObject.transform.localPosition = Vector3.zero;
        player = playerObject.AddComponent<Player>();
        player.PlayerObjectSetup(this,gameManager, debugPlayerSprite,playerPosition);

        player.health = player.data.healthMax;
        player.actionPoints = player.data.actionPointsMax;
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
        if (!enemy.isDead)
        {
            Debug.Log(enemy.health);
            ApplyCard(card, enemy);
            player.CheckForCardRemoval();
        }
        else
        {
            player.SetupCardLocation();
        }
    }
    public void ApplyCard(DeckCard card, Enemy enemy)
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
    public void ApplyCardToPlayer(DeckCard card)
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
        Debug.Log("Player have " + player.health + " hp");
    }
    void OnBattleWon()
    {
        gameManager.worldMapObject.SetActive(true);
        SceneManager.LoadScene("World");
    }
    // Update is called once per frame
    void Update()
    {
        if (inBattle)
        {
            bool isBattleWon = true;
            foreach (Enemy item in enemies)
            {
                if (!item.isDead)
                {
                    isBattleWon = false;
                }
            }
            if (isBattleWon)
            {
                OnBattleWon();
            }
            if (playerCardGiveTimer<=0f)
            {
                if (player.hand.Count<=player.data.handSize)
                {
                    player.AddCardToPlayerHand();
                    playerCardGiveTimer = playerCardGiveTime;
                }
            }
            else
            {
                playerCardGiveTimer = playerCardGiveTimer - Time.deltaTime;
            }
            if (playerResourceGiveTimer <= 0f)
            {
                player.actionPoints = player.actionPoints + 1;
                playerResourceGiveTimer = playerResourceGiveTime;
                if (player.actionPoints >player.data.actionPointsMax)
                {
                    player.actionPoints = player.data.actionPointsMax;
                }
            }
            else
            {
                playerResourceGiveTimer = playerResourceGiveTimer - Time.deltaTime;
            }
            for (int i = 0; i < enemyTimers.Count; i++)
            {
                if (enemyTimers[i]<=0)
                {
                    enemies[i].MakeAMove();
                    enemyTimers[i] = enemies[i].spellDuration;
                }
                else
                {
                    enemyTimers[i] = enemyTimers[i] - Time.deltaTime;
                }
            }
            if (effectsTimer <= 0f)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].TickEffects();
                }
                player.TickEffects();
                effectsTimer = effectsTime;
            }
            else
            {
                effectsTimer = effectsTimer - Time.deltaTime;
            }
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
