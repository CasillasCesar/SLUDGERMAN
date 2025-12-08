using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia; // Singleton para fácil acceso

    [Header("Configuración Nivel Actual")]
    public int basuraObjetivo = 5;
    public GameObject muroBloqueo; // El muro que impide pasar a la siguiente zona

    [Header("Sistema de Respawn")]
    public Transform puntoRespawnActual;
    public GameObject jugador;

    public int basuraActual = 0;

    [Header("HUD")]
    public TextMeshProUGUI textoScore; // Arrastra tu texto aquí
    public TextMeshProUGUI textoMensaje;

    void Awake()
    {
        instancia = this; // Así cualquier script puede decir GameManager.instancia.Recolectar()
    }

    void Start()
    {
        ActualizarUI();
        if (puntoRespawnActual == null)
            Debug.LogWarning("¡Asigna el Respawn Inicial en el Inspector!");
    }

    public void RecolectarBasura()
    {
        basuraActual++;
        ActualizarUI();

        // Checar si ganamos la zona
        if (basuraActual >= basuraObjetivo)
        {
            DesbloquearZona();
        }
    }

    void DesbloquearZona()
    {
        if (muroBloqueo != null)
        {
            Destroy(muroBloqueo); // ¡Adiós muro!

            // Usamos el nuevo texto central
            if (textoMensaje != null)
            {
                textoMensaje.text = "¡ZONA DESBLOQUEADA!";
                // Truco: Borrar el mensaje después de 3 segundos
                Invoke("BorrarMensaje", 3f);
            }

            // Mensaje de feedback
            if (textoScore != null) textoScore.text = "¡ZONA DESBLOQUEADA!\nCORRE...";
            Debug.Log("Muro destruido, avanza a la siguiente zona.");
        }
    }

    //void ActualizarUI()
    //{
    //    if (textoScore != null)
    //        textoScore.text = $"Basura: {basuraActual} / {basuraObjetivo}";
    //}

    public void RespawnJugador()
    {
        Debug.Log("El jugador murió - RESPAWEN");
        // Se desactiva para evitar errores por fisicas
        jugador.SetActive(false);
        // Mover al punto guardado
        jugador.transform.position = puntoRespawnActual.position;
        jugador.transform.rotation = puntoRespawnActual.rotation;

        jugador.SetActive(true);

        // Buscamos todos los enemigos que estén activos en la escena y los regresamos a su punto de origen
        EnemigoBase[] enemigosActivos = FindObjectsOfType<EnemigoBase>();

        foreach (EnemigoBase enemigo in enemigosActivos)
        {
            enemigo.ResetearPosicion();
        }
    }

    void BorrarMensaje()
    {
        if (textoMensaje != null) textoMensaje.text = "";
    }

    void ActualizarUI()
    {
        // Actualizamos solo el score
        if (textoScore != null)
            textoScore.text = $"Basura: {basuraActual} / {basuraObjetivo}";
    }
}
