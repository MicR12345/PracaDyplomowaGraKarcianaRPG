using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuExitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject resizableObject;
    public GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("World").GetComponent<GameManager>();
        RectTransform rectTransform = this.gameObject.GetComponent<RectTransform>();
        BoxCollider boxCollider = this.gameObject.GetComponent<BoxCollider>();
        if (boxCollider==null)
        {
            boxCollider = this.gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.2f);
            boxCollider.transform.localPosition = rectTransform.localPosition;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        resizableObject.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        resizableObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        gameManager.LeaveToMenu();
    }
}
