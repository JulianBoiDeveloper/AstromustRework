using UnityEngine;

public class MoveCamera3rdPerson : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float rotateSpeed = 5f;

    private float mouseX, mouseY;
    private Quaternion rotation;

    void Start()
    {
        Cursor.visible = false;
        rotation = Quaternion.Euler(0, 0, 0);
    }

    void FixedUpdate()
    {
        #if UNITY_ANDROID
            mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * rotateSpeed;
            mouseY = Mathf.Clamp(mouseY, -35f, 60f);

            rotation = Quaternion.Euler(mouseY, mouseX, 0);

            Vector3 desiredPosition = player.position + (rotation * offset);

            RaycastHit hit;
            if (Physics.Linecast(player.position, desiredPosition, out hit))
            {
                if (hit.transform != player)
                {
                    desiredPosition = hit.point + (hit.normal * 0.2f);
                }
            }

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothSpeed);
        #endif
    }
}