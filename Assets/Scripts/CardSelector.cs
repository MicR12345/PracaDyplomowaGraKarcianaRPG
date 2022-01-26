using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardSelector : MonoBehaviour
{
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    DeckCard choice1;
    DeckCard choice2;
    DeckCard choice3;
    Player player;
    GameManager gameManager;
    public void CreateCards(CardLibrary cardLibrary,Player _player,GameManager _gameManager)
    {
        player = _player;
        gameManager = _gameManager;
        choice1 = new DeckCard(cardLibrary.FindRandomCardByRarity(UnityEngine.Random.Range(0, 2)));
        choice2 = new DeckCard(cardLibrary.FindRandomCardByRarity(UnityEngine.Random.Range(0, 2)));
        choice3 = new DeckCard(cardLibrary.FindRandomCardByRarity(UnityEngine.Random.Range(0, 2)));
        choice1.card.CreateCardInstance();
        choice2.card.CreateCardInstance();
        choice3.card.CreateCardInstance();
        choice1.card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        choice2.card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        choice3.card.cardObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        choice1.card.SetSortingOrder(300);
        choice2.card.SetSortingOrder(305);
        choice3.card.SetSortingOrder(310);
        choice1.card.cardMover.moveCardTo = new Vector3(-19f, 0f, 0f);
        choice2.card.cardMover.moveCardTo = Vector3.zero;
        choice3.card.cardMover.moveCardTo = new Vector3(19f, 0f, 0f);
    }
    public void OnChoice1()
    {
        player.data.AddToPlayerDeck(choice1);
        gameManager.worldMapObject.SetActive(true);
        SceneManager.LoadScene("World");
    }
    public void OnChoice2()
    {
        player.data.AddToPlayerDeck(choice2);
        gameManager.worldMapObject.SetActive(true);
        SceneManager.LoadScene("World");
    }
    public void OnChoice3()
    {
        player.data.AddToPlayerDeck(choice3);
        gameManager.worldMapObject.SetActive(true);
        SceneManager.LoadScene("World");
    }
    public void OnSkip()
    {
        gameManager.worldMapObject.SetActive(true);
        SceneManager.LoadScene("World");
    }
}
