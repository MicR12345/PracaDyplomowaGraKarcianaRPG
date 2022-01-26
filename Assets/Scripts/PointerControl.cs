using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PointerControl : MonoBehaviour
{
    PlayerInput playerInput;
    Actions actions;

    Camera mainCamera;

    Vector2 mousePosition;
    Vector3 mouseWorldPosition;
    RaycastHit raycastHit;

    GameObject grabbedCard;
    CardHandle grabbedCardHandle;

    public BattleManager battleManager;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        mainCamera = playerInput.camera;

        actions = new Actions();
        actions.Enable();

        actions.UI.Click.performed += OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext obj)
    {
        /*Do przerobienia by reagowało na UI też*/
        if (grabbedCard == null)
        {
            Physics.Raycast(mouseWorldPosition, new Vector3(0f, 0f, 1f), out raycastHit, 10f);
            if (raycastHit.collider != null && raycastHit.collider.CompareTag("card"))
            {
                grabbedCard = raycastHit.collider.gameObject.transform.parent.gameObject;
                grabbedCardHandle = grabbedCard.GetComponent<CardHandle>();
            }
            else
            {

            }
        }
        else
        {
            Physics.Raycast(grabbedCard.transform.position, new Vector3(0f, 0f, 1f), out raycastHit, 10f);
            if (raycastHit.collider != null && raycastHit.collider.CompareTag("enemy_sprite"))
            {
                EnemyHandle enemyHandle = raycastHit.collider.gameObject.transform.parent.gameObject.GetComponent<EnemyHandle>();
                battleManager.CardWasMovedOntoEnemy(grabbedCardHandle.card, enemyHandle.enemy);
            }
            grabbedCard = null;

        }
    }
    // Update is called once per frame

    bool resetPosition = false;
    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        if (grabbedCard != null)
        {
            grabbedCardHandle.card.card.SetSortingOrder(10000);
            FancyCardMover fancyCardMover = grabbedCard.GetComponent<FancyCardMover>();
            fancyCardMover.moveCardTo = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, grabbedCard.transform.position.z);
            fancyCardMover.rotateTo = Vector3.zero;
            grabbedCard.transform.localScale = new Vector3(0.8f,0.8f,1f);
            resetPosition = true;
        }
        else if (resetPosition)
        {
            resetPosition = false;
            battleManager.player.SetupCardLocation();
        }

    }
    public void UnRegisterFromInput()
    {
        actions.UI.Click.performed -= OnClickPerformed;
    }
}
