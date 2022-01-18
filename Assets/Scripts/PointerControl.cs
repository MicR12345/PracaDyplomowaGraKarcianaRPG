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

    public BattleManager battleManager;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        mainCamera = playerInput.camera;

        actions = new Actions();
        actions.Enable();

        actions.UI.Click.performed += OnClickPerformed;
        actions.UI.Cancel.performed += Cancel_performed;
    }

    private void Cancel_performed(InputAction.CallbackContext obj)
    {
        grabbedCard = null;
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
            }
            else
            {

            }
        }
        else
        {
            //TODO
            if (grabbedCard!=null)
            {
                Physics.Raycast(grabbedCard.transform.position, new Vector3(0f, 0f, 1f), out raycastHit, 10f);
                if (raycastHit.collider != null && raycastHit.collider.CompareTag("enemy_sprite"))
                {
                    CardHandle cardHandle = grabbedCard.GetComponent<CardHandle>();
                    EnemyHandle enemyHandle = raycastHit.collider.gameObject.transform.parent.gameObject.GetComponent<EnemyHandle>();
                    battleManager.CardWasMovedOntoEnemy(cardHandle.card,enemyHandle.enemy);
                }
                grabbedCard = null;
            }
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseWorldPosition, new Vector3(0f, 0f, 0f));
    }
}
