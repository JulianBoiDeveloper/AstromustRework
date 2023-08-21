using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float mouseSensitivity = 2f;

    private Rigidbody rb;
    private bool isJumping = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + transform.TransformDirection(movement));

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.Rotate(Vector3.left * mouseY);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
}