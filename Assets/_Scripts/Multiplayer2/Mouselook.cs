using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouselook : MonoBehaviour
{
    
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;

    //private float currentZoom = 10f;

    //public float zoomSpeed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //float mouseZ -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
