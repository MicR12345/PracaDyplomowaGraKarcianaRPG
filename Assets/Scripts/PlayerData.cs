using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int charcterId;
    public int petId;
    public float healthMax = 100f;
    public int actionPointsMax = 10;
    public List<DeckCard> deck = new List<DeckCard>();
    public int deckSize = 30;
    public int handSize = 5;
    public List<Card> abilityCards;

    public PlayerData()
    {

    }
}
