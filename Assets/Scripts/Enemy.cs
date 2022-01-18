using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    public GameObject uiGameObject;
    public GameObject hpTextGameObject;

    public TextMeshPro hpText;

    public GameObject initiativeBar;

    public Slider initiativeSlider;

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
    GameObject CreateUIObject()
    {
        uiGameObject = new GameObject("UI Canvas");
        uiGameObject.transform.parent = enemyObject.transform;
        uiGameObject.transform.localPosition = Vector3.zero;
        uiGameObject.AddComponent<Canvas>();
        RectTransform canvasRT = uiGameObject.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(2.30f, 0.25f);
        return uiGameObject;
    }
    GameObject CreateHPText()
    {
        hpTextGameObject = new GameObject("HP Text");
        hpTextGameObject.transform.parent = uiGameObject.transform;
        hpTextGameObject.transform.localPosition = new Vector3(0f,-10f,0f);
        hpText = hpTextGameObject.AddComponent<TextMeshPro>();
        hpText.text = "0/0";
        return hpTextGameObject;
    }
    GameObject CreateInitiativeBar()
    {
        initiativeBar = new GameObject("Initiative Bar");
        return initiativeBar;
    }
    void CreateUIElements()
    {
        CreateUIObject();
        CreateHPText();
        CreateInitiativeBar();
    }
    public void CreateFightingEnemy(BattleManager battleManager)
    {
        CreateEnemyInstance();
        CreateEnemySpriteObject(enemySprite);
        this.battleManager = battleManager;
        enemyObject.transform.parent = battleManager.transform;
        CreateUIElements();
        
    }
    public Enemy(string _name,float _healthMax, float _spellDuration, List<string> _cardSkills,Sprite sprite)
    {
        name = _name;
        health = _healthMax;
        healthMax = _healthMax;
        spellDuration = _spellDuration;
        cardSkills = _cardSkills;

        enemySprite = sprite;
        deck = new List<DeckCard>();
        activeEffects = new List<Effect>();
    }

    public void ApplyCardEffect(Effect effect)
    {
        if (effect.name == "damage")
        {
            float damage = effect.value;
            Effect activeShield = CheckForEffect("shield");
            if (activeShield != null)
            {
                if (damage >= activeShield.value)
                {
                    damage = damage - activeShield.value;
                    RemoveEffect(activeShield);
                }
                else
                {
                    activeShield.value = activeShield.value - damage;
                    damage = 0;
                }
            }
            health = health - damage;
        }
        CheckForDeath();
    }

    public void TakeDamageDirect(float damage)
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
    public void UpdateHpBar()
    {
        hpText.text = health + "/" + healthMax;
    }
    public void MakeAMove()
    {
        List<DeckCard> possibleCard = new List<DeckCard>();
        for (int i = 0; i < deck.Count; i++)
        {
            if (!deck[0].destroyed &&
                deck[0].exhausted == 0)
            {
                possibleCard.Add(deck[0]);
            }
        }
        int pickRandom = UnityEngine.Random.Range(0, possibleCard.Count);
        Tag selfTag = possibleCard[pickRandom].card.FindCardTag("self");
        if (selfTag!=null)
        {
            battleManager.ApplyCard(possibleCard[pickRandom], this);
        }
        else
        {
            battleManager.ApplyCardToPlayer(possibleCard[pickRandom]);
        }
    }
    public Effect CheckForEffect(string name)
    {
        foreach (Effect item in activeEffects)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
    public void RemoveEffect(Effect effect)
    {
        if (activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
        }
    }
}
public class EnemyHandle : MonoBehaviour
{
    public Enemy enemy;
}
