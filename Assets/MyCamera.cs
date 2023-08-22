using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class MyCamera : MonoBehaviour
{
    [SerializeField] float yAxis;
    [SerializeField] float xAxis;
    [SerializeField] float rotationSensitivity = 8f;

    [SerializeField] float rotationMin = -22f;
    [SerializeField] float rotationMax = 80f;
    [SerializeField] float smoothTime = 0.07f;
    [SerializeField] float smoothTimePosition = 0.07f;

    [SerializeField] Transform target;
    Vector3 targetRotation;
    Vector3 currentVel;

    [SerializeField] bool enableMobileInputs = false;
    public FixedJoystick joystick;
    public FixedTouchField touchField;

    [SerializeField] float maxDistance = 6f; // Maximum distance the camera can move forward.
    private Vector3 initialOffset; // Initial offset from target to camera.
    private Vector3 desiredPosition; // Desired camera position.
    Vector3 positionSmoothDampVelocity;

    void Start()
    {
        initialOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (enableMobileInputs)
        {
            yAxis += touchField.TouchDist.x * rotationSensitivity;
            xAxis -= touchField.TouchDist.y * rotationSensitivity;
        }
        else
        {
            yAxis += Input.GetAxis("Mouse X") * rotationSensitivity;
            xAxis -= Input.GetAxis("Mouse Y") * rotationSensitivity;
        }
        xAxis = Mathf.Clamp(xAxis, rotationMin, rotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(xAxis, yAxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;

        // Calculate the desired camera position.
        desiredPosition = target.position + Quaternion.Euler(targetRotation) * initialOffset;

        // Raycast from target to camera to detect obstructions.
        RaycastHit hit;
        if (Physics.Raycast(target.position, desiredPosition - target.position, out hit, initialOffset.magnitude))
        {
            // Move the camera forward along the collision normal.
            desiredPosition = hit.point + hit.normal * 0.1f; // You might need to tweak the value.

            // Limit the movement to the maximum distance.
            if (Vector3.Distance(target.position, desiredPosition) > maxDistance)
            {
                desiredPosition = target.position + (desiredPosition - target.position).normalized * maxDistance;
            }
        }

        // Smooth the desiredPosition movement for each component.
        desiredPosition.x = Mathf.SmoothDamp(transform.position.x, desiredPosition.x, ref positionSmoothDampVelocity.x, smoothTimePosition);
        desiredPosition.y = Mathf.SmoothDamp(transform.position.y, desiredPosition.y, ref positionSmoothDampVelocity.y, smoothTimePosition);
        desiredPosition.z = Mathf.SmoothDamp(transform.position.z, desiredPosition.z, ref positionSmoothDampVelocity.z, smoothTimePosition);


        // Update the camera position.
        transform.position = desiredPosition;

        // Look at the player.
        transform.LookAt(target);
    }
}*/