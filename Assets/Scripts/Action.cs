using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public string name;
    public string description;
    public int rarity;
    public int damage;
    public int heal;
    public int cost;
    public int upgradeCount;
    public List<Effect> effects;
    public List<Tag> tags;

    public abstract void UpdateDescription();
    public abstract bool CanUpgrade();
    public abstract void UpgradeCard();
}