using UnityEngine;

public class SalidaBus : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Al tocar el bus, ganas el juego definitivamente
            if (GameManager.instancia != null)
            {
                GameManager.instancia.Victoria();
            }
        }
    }
}