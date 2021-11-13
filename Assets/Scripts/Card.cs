using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CardState
{
    active,
    inactive,
    discarded,
    burned,
    removed
}
public class Card
{
    public string name;
    public string description {
        get
        {
            UpdateDescription();
            return description;
        }
    }
    public int rarity { get; }
    public int damage;
    public int heal;
    public int cost;
    public int upgradeCount;
    public List<int> effects;

    private void UpdateDescription()
    {
        //TODO
    }

    public Card(string _name,int _damage)
    {
        name = _name;
        damage = _damage;
    }
}
