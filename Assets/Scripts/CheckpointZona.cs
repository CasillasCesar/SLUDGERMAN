using UnityEngine;

public class CheckpointZona : MonoBehaviour
{
    [Header("Jefes")]
    public GameObject jefePasado;
    public GameObject jefeDeEstaZona;

    [Header("Configuración Nueva Zona")]
    public int nuevaMetaBasura = 5;
    public GameObject siguienteMuro;
    public GeneradorBasura spawnerDeEstaZona;

    [Header("Temporizador")]
    public bool activarTiempoAqui = false; // Marcalo SOLO si empieza el Nivel 2
    public float tiempoParaEstaZona = 90f; // Segundos (ej. 1 minuto y medio)

    [Header("Respawn")]
    public Transform nuevoPuntoRespawn;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            Debug.Log("Checkpoint alcanzado.");

            // 1. Enemigos y Basura
            if (jefeDeEstaZona != null) jefeDeEstaZona.SetActive(true);
            if (jefePasado != null) jefePasado.SetActive(false);
            if (spawnerDeEstaZona != null) spawnerDeEstaZona.Generar();

            // 2. Avisar al GameManager (CON LOS NUEVOS DATOS DE TIEMPO)
            if (GameManager.instancia != null)
            {
                // Esta es la línea que te estaba dando error
                GameManager.instancia.IniciarNuevaZona(
                    nuevaMetaBasura,
                    siguienteMuro,
                    activarTiempoAqui,
                    tiempoParaEstaZona
                );

                if (nuevoPuntoRespawn != null)
                {
                    GameManager.instancia.puntoRespawnActual = nuevoPuntoRespawn;
                }
            }

            Destroy(gameObject);
        }
    }
}