using UnityEngine;

public class RotateLoader : MonoBehaviour
{
    public float rotationSpeed = 90f; // Vitesse de rotation en degrés par seconde

    // Appelé à chaque image affichée
    void Update()
    {
        // Rotation du sprite selon la vitesse spécifiée et le temps écoulé depuis le dernier frame
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
