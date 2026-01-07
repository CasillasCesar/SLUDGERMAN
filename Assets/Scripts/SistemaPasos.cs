using UnityEngine;

public class SistemaPasos : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("0.6 es caminar, 0.4 es correr")]
    public float tiempoEntrePasos = 0.6f;
    public AudioClip[] sonidosPasos;
    public AudioSource fuenteAudio;

    [Header("Ajuste de Suelo")]
    [Tooltip("Largo del rayo para detectar suelo. Si no suena, auméntalo un poco (ej. 1.1 o 1.2)")]
    public float distanciaAlSuelo = 1.1f;

    private Vector3 ultimaPosicion;
    private float cronometro;

    void Start()
    {
        if (fuenteAudio == null) fuenteAudio = GetComponent<AudioSource>();
        ultimaPosicion = transform.position;
        cronometro = 0;
    }

    void Update()
    {
        // 1. Detectar si estamos en el suelo (Raycast)
        // Lanzamos un láser desde el centro del jugador hacia abajo
        // Si choca con algo, es que estamos pisando firme.
        bool estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaAlSuelo);

        // 2. Calcular movimiento Horizontal
        Vector3 posActualPlana = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 posAnteriorPlana = new Vector3(ultimaPosicion.x, 0, ultimaPosicion.z);

        float distancia = Vector3.Distance(posActualPlana, posAnteriorPlana);
        float velocidad = distancia / Time.deltaTime;

        cronometro -= Time.deltaTime;

        // 3. CONDICIÓN TRIPLE: Moverse + Tiempo cumplido + ESTAR EN SUELO
        if (velocidad > 2.0f && cronometro <= 0 && estaEnSuelo)
        {
            ReproducirPaso();
            cronometro = tiempoEntrePasos;
        }

        ultimaPosicion = transform.position;
    }

    void ReproducirPaso()
    {
        if (sonidosPasos.Length > 0 && fuenteAudio != null)
        {
            fuenteAudio.pitch = Random.Range(0.9f, 1.1f);
            fuenteAudio.PlayOneShot(sonidosPasos[Random.Range(0, sonidosPasos.Length)]);
        }
    }

    // Dibujo visual para ver el rayo en la escena (Solo ayuda visual)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distanciaAlSuelo);
    }
}