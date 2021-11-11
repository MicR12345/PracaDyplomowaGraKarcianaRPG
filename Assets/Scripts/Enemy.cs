using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private int enemyId { get; }
    private int health { get; set; }
    private int healthMax { get; set; }
    private int moves { get; set; }
    private int movesMax { get; set; }
    private int initiative { get; set; }

    private List<Tuple<Card, CardState>> deck;
    private int deckSize { get; set; }
    private List<Card> cardStack { get; }
    private List<Card> hand { get; set; }
    private int handSize { get; set; }
    private List<Card> abilityCards { get; set; }
    private List<Tuple<int, int>> activeEffects { get; set; }
}
