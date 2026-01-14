using UnityEngine;

public class MochilaEscape : MonoBehaviour
{
    [Header("Configuración Escape")]
    public GameObject[] enemigosPerseguidores;
    public Transform puntoRespawnMochila;
    public GameObject triggerAutobus;
    public float tiempoParaHuir = 120f;

    [Header("Atmósfera (Terror)")]
    public bool activarNeblina = true;
    public Color colorNeblina = Color.gray; // Un gris oscuro o rojo queda bien
    [Range(0.01f, 0.1f)]
    public float densidadNeblina = 0.04f; // Entre 0.02 y 0.05 es lo ideal

    [Header("Música")]
    public AudioSource fuenteMusicaFondo; // Arrastra aquí el objeto que toca la música del juego
    public AudioClip musicaPersecucion;   // Arrastra aquí tu canción de escape

    [Header("Mensaje")]
    public string mensajeHuida = "¡TIENES LA MOCHILA! ¡HUYE AL INICIO!";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. CONFIGURAR GAMEMANAGER (Lógica del juego)
            if (GameManager.instancia != null)
            {
                GameManager.instancia.IniciarModoEscape(tiempoParaHuir, puntoRespawnMochila);

                if (GameManager.instancia.textoMensaje != null)
                {
                    GameManager.instancia.textoMensaje.text = mensajeHuida;
                    GameManager.instancia.textoMensaje.color = Color.red;
                }
            }

            // 2. ACTIVAR NEBLINA
            if (activarNeblina)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = colorNeblina;
                RenderSettings.fogDensity = densidadNeblina;
                RenderSettings.fogMode = FogMode.Exponential; // Se ve más realista
            }

            // 3. CAMBIAR MÚSICA
            if (fuenteMusicaFondo != null && musicaPersecucion != null)
            {
                fuenteMusicaFondo.Stop();
                fuenteMusicaFondo.clip = musicaPersecucion;
                fuenteMusicaFondo.Play();
            }

            // 4. ACTIVAR ENEMIGOS
            foreach (GameObject enemigo in enemigosPerseguidores)
            {
                if (enemigo != null) enemigo.SetActive(true);
            }

            // 5. ACTIVAR SALIDA
            if (triggerAutobus != null) triggerAutobus.SetActive(true);

            // 6. OCULTAR MOCHILA
            gameObject.SetActive(false);
        }
    }
}