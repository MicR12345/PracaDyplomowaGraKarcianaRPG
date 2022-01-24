using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    static public float playerCardGiveTime = 5f;
    float playerCardGiveTimer = playerCardGiveTime;

    static public float playerResourceGiveTime = 3f;
    float playerResourceGiveTimer = playerResourceGiveTime;

    List<float> enemyTimers;

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
    public List<Sprite> playerIdleSprite;
    public List<Sprite> playerAttackSprites;
    public List<Sprite> enemyAttackSprites;
    public List<Sprite> playerAttackDMGSprites;
    public Sprite backgroundFinalBossFight;
    public Image BgImage;
    public Vector2 playerPosition;

    List<Enemy> enemies;
    public List<Vector2> enemyPositions;
    
    [HideInInspector]
    public bool inBattle;

    public GameObject pauseBlocker;

    GameManager gameManager;

    public CardSelector cardSelector;
    void Start()
    {
        gameManager = GameObject.Find("World").GetComponent<GameManager>();
        cardLibrary = GameObject.Find("CARD LIBRARY").GetComponent<CardLibrary>();
        enemyLibrary = GameObject.Find("ENEMY LIBRARY").GetComponent<EnemyLibrary>();
        enemies = enemyLibrary.enemyList;
        
        OnBattleStart();
    }
    void OnBattleStart()
    {
        if(gameManager.bossFight == true)
        {
            BgImage.sprite = backgroundFinalBossFight;
        }
        inBattle = true;
        if (player==null)
        {
            CreatePlayerObject();
        }
        enemies = new List<Enemy>();
        enemies = gameManager.enemiesInBattle;
        enemyTimers = new List<float>();
        foreach (Enemy enemy in enemies)
        {
            enemy.CreateFightingEnemy(this);
            enemyTimers.Add(enemy.spellDuration);
        }
        PlaceEnemies();

        player.actionPoints = player.data.actionPointsMax;

        //CheatGiveDebugCardsToDeck(10);
        player.PrepareHandBeforeBattle();
        player.DealAFullHand();
        player.SetupCardLocation();
        player.UpdateApDisplay();
        player.UpdateHpBar();
    }
    void CreatePlayerObject()
    {
        playerObject = new GameObject();
        playerObject.transform.parent = this.gameObject.transform;
        playerObject.transform.localPosition = Vector3.zero;
        player = playerObject.AddComponent<Player>();
        player.PlayerObjectSetup(this,gameManager, playerIdleSprite,playerPosition);

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
        if (!enemy.isDead && player.actionPoints>= card.card.cost)
        {
            player.actionPoints = player.actionPoints - card.card.cost;
            player.UpdateApDisplay();
            ApplyCard(card, enemy);
            player.CheckForCardRemoval();
            player.spriteAnimator.PlayOnce(playerAttackSprites, 0.1f);
            Vector3 position = enemy.enemyObject.transform.position;
            player.CreateDMGAnimation(playerAttackDMGSprites, position + new Vector3(0f, 0f, 0.5f));
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
        inBattle = false;
        player.HideAllHand();
        pointerControl.UnRegisterFromInput();
        if (gameManager.bossFight)
        {
            gameManager.WinGame();
        }
        else
        {
            cardSelector.CreateCards(cardLibrary, player,gameManager);
            cardSelector.gameObject.SetActive(true);
        }
        
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
                if (player.hand.Count<player.data.handSize)
                {
                    player.AddCardToPlayerHand();
                    playerCardGiveTimer = playerCardGiveTime;
                    player.UpdateCardBar(playerCardGiveTimer);
                }
            }
            else
            {
                playerCardGiveTimer = playerCardGiveTimer - Time.deltaTime;
                player.UpdateCardBar(playerCardGiveTimer);
            }
            if (playerResourceGiveTimer <= 0f)
            {
                player.actionPoints = player.actionPoints + 1;
                playerResourceGiveTimer = playerResourceGiveTime;
                if (player.actionPoints >player.data.actionPointsMax)
                {
                    player.actionPoints = player.data.actionPointsMax;
                }
                player.UpdateApDisplay();
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
                    enemies[i].CreateDMGAnimation(enemyAttackSprites, playerPosition);
                    enemyTimers[i] = enemies[i].spellDuration;
                    enemies[i].UpdateInitiativeBar(enemyTimers[i]);
                }
                else
                {
                    enemyTimers[i] = enemyTimers[i] - Time.deltaTime;
                    enemies[i].UpdateInitiativeBar(enemyTimers[i]);
                }
            }
            foreach (Enemy item in enemies)
            {
                item.UpdateHpBar();
            }
            player.UpdateHpBar();
        }

    }
    public void PauseGame()
    {
        pauseBlocker.SetActive(true);
        inBattle = false;
    }
    public void ResumeGame()
    {
        pauseBlocker.SetActive(false);
        inBattle = true;
    }
    //DEBUGGING FUNCTIONS

    void CheatGiveDebugCardsToDeck(int count)
    {
        for (int i = 0; i < 10; i++)
        {
            player.data.AddToPlayerDeck(new DeckCard(cardLibrary.FindCardByName("0_cardTest")));
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
