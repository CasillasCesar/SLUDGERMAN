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

    [Header("Paneles UI")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;
    public GameObject panelPausa; // <--- NUEVO: Arrastra aquí tu panel de pausa

    // Variables Internas
    private int vidasActuales;
    private float tiempoRestante = 60f;
    private bool cuentaRegresivaActiva = false;
    private bool juegoTerminado = false;
    private bool juegoPausado = false; // <--- NUEVO: Control de pausa

    // MEMORIA
    private float tiempoInicialDeZona = 0f;
    private GeneradorBasura spawnerActual;
    private bool zonaYaDesbloqueada = false;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        vidasActuales = vidasMaximas;

        // Aseguramos que los paneles estorben al inicio
        if (panelGameOver) panelGameOver.SetActive(false);
        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelPausa) panelPausa.SetActive(false); // <--- NUEVO

        if (textoTiempo) textoTiempo.text = "";

        ActualizarUI();
    }

    void Update()
    {
        // --- NUEVO: SISTEMA DE PAUSA ---
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePausa();
        }

        if (juegoTerminado || juegoPausado) return; // Si está pausado, no corre el tiempo
        // -------------------------------

        if (cuentaRegresivaActiva)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante < 0) tiempoRestante = 0;

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

    // --- NUEVO: FUNCIÓN DE PAUSA ---
    public void TogglePausa()
    {
        if (juegoTerminado) return; // No pausar si ya moriste o ganaste

        juegoPausado = !juegoPausado;

        if (juegoPausado)
        {
            Time.timeScale = 0f; // Congela el tiempo (física, animaciones, timers)
            if (panelPausa) panelPausa.SetActive(true);

            // Liberar mouse para usar el menú
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Opcional: Pausar música si quieres
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f; // Tiempo normal
            if (panelPausa) panelPausa.SetActive(false);

            // Bloquear mouse para seguir jugando
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            AudioListener.pause = false;
        }
    }

    public void RecibirDano(int cantidad)
    {
        if (juegoTerminado) return;

        vidasActuales -= cantidad;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (i < vidasActuales) corazones[i].SetActive(true);
            else corazones[i].SetActive(false);
        }

        if (vidasActuales <= 0) GameOver();
    }

    public void RecolectarBasura()
    {
        if (juegoTerminado) return;

        basuraActual++;
        ActualizarUI();

        if (basuraActual >= basuraObjetivo && !zonaYaDesbloqueada)
        {
            DesbloquearZona();
        }
    }

    public void IniciarNuevaZona(int nuevaMeta, GameObject nuevoMuro, bool activarTiempo, float tiempoZona, GeneradorBasura spawner)
    {
        basuraActual = 0;
        basuraObjetivo = nuevaMeta;
        muroBloqueo = nuevoMuro;
        spawnerActual = spawner;

        zonaYaDesbloqueada = false;

        if (activarTiempo)
        {
            tiempoRestante = tiempoZona;
            tiempoInicialDeZona = tiempoZona;
            cuentaRegresivaActiva = true;
        }
        else
        {
            tiempoInicialDeZona = 0;
            cuentaRegresivaActiva = false;
            if (textoTiempo) textoTiempo.text = "";
        }

        ActualizarUI();
    }

    void DesbloquearZona()
    {
        zonaYaDesbloqueada = true;

        if (muroBloqueo != null)
        {
            muroBloqueo.SetActive(false);
            if (textoMensaje != null)
            {
                textoMensaje.text = "¡ZONA DESBLOQUEADA!";
                textoMensaje.color = Color.green;
                Invoke("BorrarMensaje", 3f);
            }
        }
        else
        {
            Victoria();
        }
    }

    public void IniciarModoEscape(float tiempoEscape, Transform nuevoRespawn)
    {
        if (muroBloqueo != null) muroBloqueo.SetActive(false);

        muroBloqueo = null;
        spawnerActual = null;
        basuraActual = 0;
        zonaYaDesbloqueada = true;

        if (tiempoEscape > 0)
        {
            tiempoInicialDeZona = tiempoEscape;
            tiempoRestante = tiempoEscape;
            cuentaRegresivaActiva = true;
        }

        if (nuevoRespawn != null) puntoRespawnActual = nuevoRespawn;
        ActualizarUI();
    }

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
        CongelarJugador();

        if (panelGameOver) panelGameOver.SetActive(true);
        if (textoMensaje) textoMensaje.text = "¡FIN DEL JUEGO!";
    }

    public void Victoria()
    {
        juegoTerminado = true;
        cuentaRegresivaActiva = false;
        CongelarJugador();

        if (panelVictoria) panelVictoria.SetActive(true);
    }

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

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; // IMPORTANTE: Descongelar tiempo antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f; // IMPORTANTE: Descongelar tiempo antes de salir
        SceneManager.LoadScene(0); // Carga la Escena 0 (Menú Principal)
    }

    void BorrarMensaje() { if (textoMensaje != null) textoMensaje.text = ""; }
    void ActualizarUI() { if (textoScore != null) textoScore.text = $"Basura: {basuraActual} / {basuraObjetivo}"; }

    public void ReiniciarDesdeCheckpoint()
    {
        juegoTerminado = false;
        vidasActuales = vidasMaximas;
        basuraActual = 0;
        if (textoMensaje != null) textoMensaje.text = "";
        ActualizarUI();

        if (spawnerActual != null) spawnerActual.ResetearSistema();

        if (muroBloqueo != null)
        {
            muroBloqueo.SetActive(true);
            zonaYaDesbloqueada = false;
        }

        if (tiempoInicialDeZona > 0)
        {
            tiempoRestante = tiempoInicialDeZona;
            cuentaRegresivaActiva = true;
        }
        else
        {
            cuentaRegresivaActiva = false;
            if (textoTiempo) textoTiempo.text = "";
        }

        if (panelGameOver) panelGameOver.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (jugador != null)
        {
            jugador.GetComponent<MovimientoSimple>().enabled = true;
            jugador.GetComponent<SistemaPasos>().enabled = true;
            jugador.GetComponent<InteraccionJugador>().enabled = true;
        }

        ActualizarCorazonesUI();
        RespawnJugador();
    }

    void ActualizarCorazonesUI()
    {
        for (int i = 0; i < corazones.Length; i++) corazones[i].SetActive(true);
    }
}