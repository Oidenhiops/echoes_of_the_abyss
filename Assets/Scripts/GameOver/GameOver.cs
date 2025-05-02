using UnityEngine;

public class GameOver : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("¡Colisión detectada con: " + collision.gameObject.name + "!");
        Destroy(gameObject);
    }
}
