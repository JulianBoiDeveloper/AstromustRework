using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class OnShootDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] FixedTouchField fixedTouchField;
    [SerializeField] TestCharacterController characterController;
    float shootTimer = 0f;
    float shootDuration = 0.12f;

    bool isPressed = false;

    void Update()
    {
        if(isPressed)
        {
            shootTimer += Time.deltaTime;

            if(shootTimer > shootDuration) {
                characterController.TryInteract(false);
                shootTimer = 0f;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        fixedTouchField.Pressed = true;
        isPressed = true;
        fixedTouchField.ControlFromShoot(eventData);
  //      characterController.TryInteract();

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        shootTimer = shootDuration;
        fixedTouchField.Pressed = false;
        isPressed = false;
    }

}
