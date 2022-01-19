using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public float healthMax = 100f;
    public int actionPointsMax = 10;
    public List<DeckCard> deck = new List<DeckCard>();
    public int deckSize = 30;
    public int handSize = 5;

    public PlayerData()
    {
        deck = new List<DeckCard>();
    }
    public PlayerData(SaveManager.PlayerSaveData playerSaveData,CardLibrary cardLibrary)
    {
        deck = new List<DeckCard>();
        healthMax = playerSaveData.healthMax;
        actionPointsMax = playerSaveData.actionPointsMax;
        deckSize = playerSaveData.deckSize;
        handSize = playerSaveData.handSize;
        foreach (SaveManager.DeckCardData item in playerSaveData.deck.deckCards)
        {
            DeckCard toBeAddedCard = new DeckCard(cardLibrary.FindCardByName(item.name));
            toBeAddedCard.card.rarity = item.rarity;
            toBeAddedCard.card.cost = item.cost;
            toBeAddedCard.card.effects = new List<Effect>(item.effect);
            toBeAddedCard.card.tags = new List<Tag>(item.tag);
            deck.Add(toBeAddedCard);
        }
    }
}
