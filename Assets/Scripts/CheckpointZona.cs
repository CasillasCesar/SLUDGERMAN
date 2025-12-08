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

                if (jefePasado)
                {
                    jefePasado.SetActive(false);
                }
            }

            // 2. Reiniciar el GameManager para la nueva misión
            GameManager.instancia.basuraObjetivo = nuevaMetaBasura;
            GameManager.instancia.muroBloqueo = siguienteMuro;
            // Resetear contador interno (opcional, o acumularlo)

            // 3. Destruirse para no activarse doble
            Destroy(gameObject);
        }
    }
}
