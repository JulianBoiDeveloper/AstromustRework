using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CubeMovement : MonoBehaviour
{
    public float speed = 5f;

    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!view.IsMine) return;

        // Move the cube based on user input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the movement direction
        Vector3 movement = new Vector3(-moveHorizontal, 0f, -moveVertical);

        // Apply the movement to the cube's position
        transform.position += movement * speed * Time.deltaTime;
    }
}
