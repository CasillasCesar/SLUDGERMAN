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
    public GeneradorBasura spawnerDeEstaZona; 

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
            if (spawnerDeEstaZona != null)
            {
                spawnerDeEstaZona.Generar(); // <--- ¡Esta línea hace la magia!
            }

            // 2. Actualizar GameManager
            if (GameManager.instancia != null)
            {
                GameManager.instancia.IniciarNuevaZona(nuevaMetaBasura, siguienteMuro);

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
