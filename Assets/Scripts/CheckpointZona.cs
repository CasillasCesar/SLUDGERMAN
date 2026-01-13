using UnityEngine;

public class CheckpointZona : MonoBehaviour
{
    [Header("Configuración de Zona")]
    public int nuevaMetaBasura = 5;
    public GameObject siguienteMuro;

    // ESTA ES LA VARIABLE QUE FALTABA ENVIAR
    public GeneradorBasura spawnerDeEstaZona;

    [Header("Enemigos (Jefes)")]
    public GameObject jefePasado;
    public GameObject jefeDeEstaZona;

    [Header("Temporizador")]
    public bool activarTiempoAqui = false;
    public float tiempoParaEstaZona = 90f;

    [Header("Respawn")]
    public Transform nuevoPuntoRespawn;

    private bool activado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            activado = true;
            Debug.Log("Entraste a la caja invisible: Activando Zona.");

            // 1. Configurar Enemigos
            if (jefeDeEstaZona != null) jefeDeEstaZona.SetActive(true);
            if (jefePasado != null) jefePasado.SetActive(false);

            // 2. GENERAR BASURA AHORA
            // Como el jugador acaba de llegar, generamos la basura de esta zona
            if (spawnerDeEstaZona != null)
            {
                // Usamos ResetearSistema en lugar de Generar directo para asegurar limpieza
                spawnerDeEstaZona.ResetearSistema();
            }

            // 3. AVISAR AL GAMEMANAGER
            if (GameManager.instancia != null)
            {
                // --- AQUÍ ESTABA EL ERROR ---
                // Ahora le pasamos el 5to argumento: spawnerDeEstaZona
                GameManager.instancia.IniciarNuevaZona(
                    nuevaMetaBasura,
                    siguienteMuro,
                    activarTiempoAqui,
                    tiempoParaEstaZona,
                    spawnerDeEstaZona // <--- ESTO FALTABA
                );

                if (nuevoPuntoRespawn != null)
                {
                    GameManager.instancia.puntoRespawnActual = nuevoPuntoRespawn;
                }
            }

            // Destruimos la caja invisible para no activarla dos veces
            Destroy(gameObject);
        }
    }
}