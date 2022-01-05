using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy
{
    public string name;
    public float health;
    public float healthMax;
    public int moves;
    public int movesMax;
    public int initiative;

    public int size;

    public bool isDead = false;

    public List<DeckCard> deck;
    public List<DeckCard> abilityCards;
    public List<Effect> activeEffects;

    public GameObject enemyObject;
    public GameObject spriteObject;

    public Sprite enemySprite;

    public GameManager gameManager;

    public Enemy Clone()
    {
        Enemy enemy = (Enemy)this.MemberwiseClone();
        enemy.enemyObject = CreateEnemyInstance();
        enemy.spriteObject = CreateEnemySpriteObject(enemySprite);
        return enemy;
    }
    
    GameObject CreateEnemyInstance()
    {
        if (enemyObject==null)
        {
            enemyObject = new GameObject(name);
            enemyObject.transform.parent = gameManager.transform;
            enemyObject.transform.localPosition = Vector3.zero;
            EnemyHandle enemyHandle = enemyObject.AddComponent<EnemyHandle>();
            enemyHandle.enemy = this;
            enemyObject.tag = "enemy";
        }
        else
        {
            Debug.LogError("Trying to create enemy that alredy exists");
        }
        return null;
    }

    GameObject CreateEnemySpriteObject(Sprite sprite)
    {
        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = enemyObject.transform;
        spriteObject.transform.localPosition = Vector3.zero;
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        BoxCollider collider = spriteObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(12f,39f,0.2f);
        spriteObject.tag = "enemy_sprite";
        return spriteObject;
    }
    public Enemy(GameManager _gameManager,string _name,float _healthMax,int _movesMax,int _initiative,int _size,Sprite sprite)
    {
        gameManager = _gameManager;

        name = _name;
        health = _healthMax;
        healthMax = _healthMax;
        moves = _movesMax;
        movesMax = _movesMax;
        initiative = _initiative;
        size = _size;

        enemySprite = sprite;

        CreateEnemyInstance();
        CreateEnemySpriteObject(sprite);
    }

    public void ApplyCardEffect(Effect effect)
    {
        if (effect.name == "damage")
        {
            health = health - effect.value;
        }
        CheckForDeath();
    }

    public void TakeDamageDirect()
    {

    }

    public void TickEffects()
    {

    }

    public void SetPosition(Vector2 position)
    {
        enemyObject.transform.localPosition = position;
    }
    public void CheckForDeath()
    {
        if (health<=0)
        {
            isDead = true;
            enemyObject.transform.Rotate(Vector3.forward, 90);
        }
    }
    public void MakeAMove()
    {
        //TODO
        //AI
    }
}
public class EnemyHandle : MonoBehaviour
{
    public Enemy enemy;
}
