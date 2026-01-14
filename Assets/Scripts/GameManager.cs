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

        if (panelGameOver) panelGameOver.SetActive(false);
        if (panelVictoria) panelVictoria.SetActive(false);
        if (textoTiempo) textoTiempo.text = "";

        ActualizarUI();
    }

    void Update()
    {
        if (juegoTerminado) return;

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
        else Debug.Log("Sigues vivo");
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

        // --- CAMBIO AQUÍ: QUITAMOS LA PAUSA DEL RELOJ ---
        // cuentaRegresivaActiva = false; (Línea borrada para que el tiempo siga corriendo)

        if (muroBloqueo != null)
        {
            muroBloqueo.SetActive(false); // Ocultar muro

            if (textoMensaje != null)
            {
                textoMensaje.text = "¡ZONA DESBLOQUEADA!";
                textoMensaje.color = Color.green;
                Invoke("BorrarMensaje", 3f);
            }
        }
        else
        {
            // Solo si ganamos el juego completo detenemos el tiempo
            Victoria();
        }
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
        cuentaRegresivaActiva = false; // Aquí SI se detiene
        CongelarJugador();

        if (panelGameOver) panelGameOver.SetActive(true);
        if (textoMensaje) textoMensaje.text = "¡FIN DEL JUEGO!";
    }

    public void Victoria()
    {
        juegoTerminado = true;
        cuentaRegresivaActiva = false; // Aquí SI se detiene
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

    public void ReiniciarNivel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void SalirAlMenu() { SceneManager.LoadScene(0); }
    void BorrarMensaje() { if (textoMensaje != null) textoMensaje.text = ""; }
    void ActualizarUI() { if (textoScore != null) textoScore.text = $"Basura: {basuraActual} / {basuraObjetivo}"; }

    public void ReiniciarDesdeCheckpoint()
    {
        Debug.Log("Reviviendo...");

        juegoTerminado = false;
        vidasActuales = vidasMaximas;
        basuraActual = 0;

        if (textoMensaje != null) textoMensaje.text = "";

        ActualizarUI();

        if (spawnerActual != null)
        {
            spawnerActual.ResetearSistema();
        }

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
            var movimiento = jugador.GetComponent<MovimientoSimple>();
            if (movimiento != null) movimiento.enabled = true;
            var pasos = jugador.GetComponent<SistemaPasos>();
            if (pasos != null) pasos.enabled = true;
            var interaccion = jugador.GetComponent<InteraccionJugador>();
            if (interaccion != null) interaccion.enabled = true;
        }

        ActualizarCorazonesUI();
        RespawnJugador();
    }

    void ActualizarCorazonesUI()
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].SetActive(true);
        }
    }

    public void IniciarModoEscape(float tiempoEscape, Transform nuevoRespawn)
    {
        Debug.Log("¡MODO ESCAPE ACTIVADO!");

        // 1. Limpiar rastro del nivel anterior
        // Si había un muro activo, lo apagamos para que no estorbe al regresar
        if (muroBloqueo != null)
        {
            muroBloqueo.SetActive(false);
        }

        // Vaciamos las variables para que al morir NO intente poner muro ni basura
        muroBloqueo = null;
        spawnerActual = null;

        basuraActual = 0;
        basuraObjetivo = 0; // Ya no hay que juntar nada
        zonaYaDesbloqueada = true; // Para que no intente desbloquear nada

        // 2. Configurar el Nuevo Tiempo
        if (tiempoEscape > 0)
        {
            tiempoInicialDeZona = tiempoEscape; // Guardamos el tiempo de escape (ej. 120s)
            tiempoRestante = tiempoEscape;
            cuentaRegresivaActiva = true;
        }
        else
        {
            cuentaRegresivaActiva = false;
        }

        // 3. Actualizar Respawn
        if (nuevoRespawn != null)
        {
            puntoRespawnActual = nuevoRespawn;
        }

        ActualizarUI();
    }
}