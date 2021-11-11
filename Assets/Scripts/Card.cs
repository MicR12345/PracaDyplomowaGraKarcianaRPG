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
    private int cardId { get; }
    private string name { 
        get 
        {
            return name;
        } 
    }
    private string description {
        get
        {
            UpdateDescription();
            return description;
        }
    }
    private int rarity { get; }
    private int damage { get; }
    private int heal { get; }
    private int cost { get; }
    private int upgradeCount { get; set; }
    private List<int> effects;

    private void UpdateDescription()
    {
        //TODO
    }
}
