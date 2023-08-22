using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookCamera : MonoBehaviour
{
    public Transform player;
    public float sensitivity = 2f;

    private float mouseX, mouseY;
    private float rotationX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        rotationX = mouseY;

        Quaternion targetRotation = Quaternion.Euler(rotationX, mouseX, 0f);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}
