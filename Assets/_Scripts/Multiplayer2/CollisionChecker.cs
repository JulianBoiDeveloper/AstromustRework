using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public bool IsColliding { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the GameObject is colliding with another GameObject
        IsColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the collision with another GameObject has ended
        IsColliding = false;
    }
}