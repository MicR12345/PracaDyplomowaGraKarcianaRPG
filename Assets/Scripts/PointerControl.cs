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
            if (raycastHit.collider != null && raycastHit.collider.CompareTag("card_sprite"))
            {
                Debug.Log(mouseWorldPosition);
                grabbedCard = raycastHit.collider.gameObject;
            }
        }
        else
        {
            //TODO
            grabbedCard = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        if (grabbedCard != null)
        {
            grabbedCard.transform.position = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, grabbedCard.transform.position.z);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(mouseWorldPosition, new Vector3(0f, 0f, 0f));
    }
}
