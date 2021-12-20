using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Do usunięcia wszystkie get sety bo unity ich nie lubi
public class Enemy
{
    private int enemyId;
    private int health;
    private int healthMax;
    private int moves;
    private int movesMax;
    private int initiative;

    //private List<Tuple<Card, CardState>> deck;
    private int deckSize;
    private List<Card> cardStack;
    private List<Card> hand;
    private int handSize;
    private List<Card> abilityCards;
    private List<Tuple<int, int>> activeEffects;
}
