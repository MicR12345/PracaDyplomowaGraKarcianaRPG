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

    public GameManager gameManager;
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
        gameManager.player.AddCardToPlayerHand();
        /*if (gm.player.hand.Count < 3)
        {
            gm.player.AddCardToPlayerHand();
        }
        else
        {
            gm.player.RemoveAllFromHand();
            gm.player.Shuffle();
        }*/
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
                    gameManager.CardWasMovedOntoEnemy(cardHandle.card,enemyHandle.enemy);
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
            grabbedCard.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, grabbedCard.transform.position.z);
            resetPosition = true;
        }
        else if (resetPosition)
        {
            resetPosition = false;
            gameManager.player.SetupCardLocation();
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseWorldPosition, new Vector3(0f, 0f, 0f));
    }
}
