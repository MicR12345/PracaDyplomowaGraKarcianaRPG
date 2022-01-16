using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy
{
    public string name;
    public float health;
    public float healthMax;
    public float spellDuration;
    public List<string> cardSkills;

    public bool isDead = false;

    public List<DeckCard> deck;
    public List<Effect> activeEffects;

    public GameObject enemyObject;
    public GameObject spriteObject;

    public Sprite enemySprite;

    public BattleManager battleManager;

    public Enemy Clone()
    {
        Enemy enemy = (Enemy)this.MemberwiseClone();
        return enemy;
    }
    GameObject CreateEnemyInstance()
    {
        if (enemyObject == null)
        {
            enemyObject = new GameObject(name);
            enemyObject.transform.localPosition = Vector3.zero;
            EnemyHandle enemyHandle = enemyObject.AddComponent<EnemyHandle>();
            enemyHandle.enemy = this;
            enemyObject.tag = "enemy";
            return enemyObject;
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
        collider.size = new Vector3(12f, 39f, 0.2f);
        spriteObject.tag = "enemy_sprite";
        return spriteObject;
    }
    public void CreateFightingEnemy(BattleManager battleManager)
    {
        CreateEnemyInstance();
        CreateEnemySpriteObject(enemySprite);
        this.battleManager = battleManager;
        enemyObject.transform.parent = battleManager.transform;
        
    }
    public Enemy(string _name,float _healthMax, float _spellDuration, List<string> _cardSkills,Sprite sprite)
    {
        name = _name;
        health = _healthMax;
        healthMax = _healthMax;
        spellDuration = _spellDuration;
        cardSkills = _cardSkills;

        enemySprite = sprite;

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
