using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int charcterId { get; }
    private int petId { get { return petId; } }
    private int healthMax { get; set; }
    private int health { get; set; }
    private int moves { get; set; }
    private int movesMax { get; set; }
    private int initiative { get; set; }
    private List<Tuple<Card, CardState>> deck;
    private int deckSize { get; set; }
    private List<Card> cardStack { get; }
    private int stackKnowledge { get; set; }
    private List<Card> hand { get; set; }
    private int handSize { get; set; }
    private List<Card> abilityCards { get; set; }
    private List<Tuple<int,int>> activeEffects { get; set; }
    
}
