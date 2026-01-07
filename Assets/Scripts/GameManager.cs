using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Configuración Nivel Actual")]
    public int basuraObjetivo = 5;
    public GameObject muroBloqueo;

    [Header("Sistema de Respawn")]
    public Transform puntoRespawnActual;
    public GameObject jugador;

    [Header("Estado del Juego")]
    public int vidasMaximas = 3;
    public int basuraActual = 0;

    [Header("HUD")]
    public TextMeshProUGUI textoScore;
    public TextMeshProUGUI textoMensaje;
    public TextMeshProUGUI textoTiempo;
    public GameObject[] corazones;
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    // Variables Internas
    private int vidasActuales;
    private float tiempoRestante = 60f;
    private bool cuentaRegresivaActiva = false;
    private bool juegoTerminado = false;

    // --- NUEVO: CANDADO PARA EVITAR GANAR POR ERROR ---
    private bool zonaYaDesbloqueada = false;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        vidasActuales = vidasMaximas;

        if (panelGameOver) panelGameOver.SetActive(false);
        if (panelVictoria) panelVictoria.SetActive(false);
        if (textoTiempo) textoTiempo.text = "";

        ActualizarUI();
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Temporizador
        if (cuentaRegresivaActiva)
        {
            tiempoRestante -= Time.deltaTime;
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);

            if (textoTiempo) textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                GameOver();
            }
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (juegoTerminado) return;

        vidasActuales -= cantidad;
        Debug.Log($"Vidas restantes: {vidasActuales}");

        for (int i = 0; i < corazones.Length; i++)
        {
            if (i < vidasActuales) corazones[i].SetActive(true);
            else corazones[i].SetActive(false);
        }

        if (vidasActuales <= 0) GameOver();
        else RespawnJugador();
    }

    // --- AQUÍ ESTÁ EL ARREGLO ---
    public void RecolectarBasura()
    {
        if (juegoTerminado) return;

        basuraActual++;
        ActualizarUI();

        // Solo intentamos desbloquear SI NO lo hemos hecho ya
        if (basuraActual >= basuraObjetivo && !zonaYaDesbloqueada)
        {
            DesbloquearZona();
        }
    }

    // --- TRANSICIÓN DE ZONAS ---
    public void IniciarNuevaZona(int nuevaMeta, GameObject nuevoMuro, bool activarTiempo, float tiempoZona)
    {
        basuraActual = 0;
        basuraObjetivo = nuevaMeta;
        muroBloqueo = nuevoMuro;

        // --- IMPORTANTE: Reseteamos el candado para la nueva zona ---
        zonaYaDesbloqueada = false;

        if (activarTiempo)
        {
            tiempoRestante = tiempoZona;
            cuentaRegresivaActiva = true;
        }

        ActualizarUI();
    }

    void DesbloquearZona()
    {
        // Ponemos el candado para que la basura extra no vuelva a activar esto
        zonaYaDesbloqueada = true;

        if (muroBloqueo != null)
        {
            Destroy(muroBloqueo);

            if (textoMensaje != null)
            {
                textoMensaje.text = "¡ZONA DESBLOQUEADA!";
                textoMensaje.color = Color.green;
                Invoke("BorrarMensaje", 3f);
            }
        }
        else
        {
            // Solo ganamos si NO hay muro Y acabamos de cumplir la meta
            Victoria();
        }
    }

    // --- FUNCIONES DE ESTADO ---
    public void RespawnJugador()
    {
        jugador.SetActive(false);
        jugador.transform.position = puntoRespawnActual.position;
        jugador.transform.rotation = puntoRespawnActual.rotation;
        jugador.SetActive(true);

        EnemigoBase[] enemigosActivos = FindObjectsByType<EnemigoBase>(FindObjectsSortMode.None);
        foreach (EnemigoBase enemigo in enemigosActivos) enemigo.ResetearPosicion();
    }

    public void GameOver()
    {
        juegoTerminado = true;
        cuentaRegresivaActiva = false;
        CongelarJugador(); // Función nueva para limpiar código

        if (panelGameOver) panelGameOver.SetActive(true);
        if (textoMensaje) textoMensaje.text = "";
    }

    public void Victoria()
    {
        juegoTerminado = true;
        cuentaRegresivaActiva = false;
        CongelarJugador(); // Función nueva para limpiar código

        if (panelVictoria) panelVictoria.SetActive(true);
    }

    // --- AUXILIAR PARA DETENER TODO ---
    void CongelarJugador()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (jugador != null)
        {
            var movimiento = jugador.GetComponent<MovimientoSimple>();
            if (movimiento != null) movimiento.enabled = false;

            var pasos = jugador.GetComponent<SistemaPasos>();
            if (pasos != null) pasos.enabled = false;

            var interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null) interaccion.enabled = false;
        }
    }

    public void ReiniciarNivel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void SalirAlMenu() { SceneManager.LoadScene(0); }
    void BorrarMensaje() { if (textoMensaje != null) textoMensaje.text = ""; }
    void ActualizarUI() { if (textoScore != null) textoScore.text = $"Basura: {basuraActual} / {basuraObjetivo}"; }
}