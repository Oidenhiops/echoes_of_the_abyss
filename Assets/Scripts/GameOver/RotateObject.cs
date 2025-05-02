using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocidad de rotación en grados por segundo

    void Update()
    {
        // Rotar continuamente sobre el eje Y
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
