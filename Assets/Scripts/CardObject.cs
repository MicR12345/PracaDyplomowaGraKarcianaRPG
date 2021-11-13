using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    Sprite cardImage;
    Sprite cardBorder;
    public Card card;

    public CardObject(Sprite _cardImage,Sprite _cardBorder,Card _card)
    {
        cardImage = _cardImage;
        cardBorder = _cardBorder;
        card = _card;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
