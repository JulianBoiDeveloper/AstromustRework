using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 1f, -3f);
    public float smoothSpeed = 10f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = player.position + player.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, smoothSpeed * Time.deltaTime);
    }
}
