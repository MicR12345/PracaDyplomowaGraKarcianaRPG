using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int charcterId;
    public int petId;
    public int healthMax;
    public int actionPointsMax;
    public int initiative;
    public List<DeckCard> deck;
    public int deckSize = 30;
    public int stackKnowledge;
    public int handSize = 5;
    public List<Card> abilityCards;

    public PlayerData()
    {

    }
}
