using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int number;
    public CardSelector cardSelector;
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (number == 0)
        {
            cardSelector.OnSkip();
        }
        if (number == 1)
        {
            cardSelector.OnChoice1();
        }
        if (number == 2)
        {
            cardSelector.OnChoice2();
        }
        if (number == 3)
        {
            cardSelector.OnChoice3();
        }
    }
}
