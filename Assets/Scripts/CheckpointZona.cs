using UnityEngine;

public class CheckpointZona : MonoBehaviour
{
    [Header("¿Hay algún enemigo que eliminar?")]
    public GameObject jefePasado;

    [Header("¿A quién despertamos?")]
    public GameObject jefeDeEstaZona;

    [Header("Configuración Nueva Zona")]
    public int nuevaMetaBasura = 5;
    public GameObject siguienteMuro;

    [Header("Respawn")]
    public Transform nuevoPuntoRespawn;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            Debug.Log("¡Entrando a Zona Peligrosa!");

            // 1. Activar al Jefe
            if (jefeDeEstaZona != null)
            {
                jefeDeEstaZona.SetActive(true);

                if (jefePasado && jefePasado.activeSelf)
                {
                    jefePasado.SetActive(false);
                }
            }

            // 2. Reiniciar el GameManager para la nueva misión
            GameManager.instancia.basuraObjetivo = nuevaMetaBasura;
            GameManager.instancia.muroBloqueo = siguienteMuro;
            // Resetear contador interno (opcional, o acumularlo)

            // 2. Actualizar GameManager
            if (GameManager.instancia != null)
            {
                GameManager.instancia.basuraObjetivo = nuevaMetaBasura;
                GameManager.instancia.muroBloqueo = siguienteMuro;

                // ACTUALIZAR EL RESPAWN y el jefe
                if (nuevoPuntoRespawn != null)
                {
                    GameManager.instancia.puntoRespawnActual = nuevoPuntoRespawn;
                }
            }

            // 3. Destruirse para no activarse doble
            Destroy(gameObject);
        }
    }
}
