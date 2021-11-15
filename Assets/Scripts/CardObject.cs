using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    Sprite cardImage;
    Sprite cardBorder;
    public Card card;

    SpriteRenderer spriteRenderer;
    SpriteRenderer borderRenderer;

    GameObject spriteObject;
    GameObject borderObject;

    public void SetupCardPrototype(GameObject _go,Sprite _cardImage, Sprite _cardBorder, Card _card)
    {
        cardImage = _cardImage;
        cardBorder = _cardBorder;
        card = _card;

        CreateSpriteChild();
        //CreateBorderChild();
        
        gameObject.SetActive(false);
    }
    public void CreateCard()
    {
        gameObject.SetActive(true);
        
    }
    void CreateSpriteChild()
    {
        spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = gameObject.transform;
        spriteObject.transform.localPosition = Vector3.zero;
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardImage;
    }
    void CreateBorderChild()
    {
        borderObject = new GameObject("Border");
        borderObject.transform.parent = gameObject.transform;
        borderObject.transform.localPosition = Vector3.zero;
        borderRenderer = spriteObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = cardBorder;
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
