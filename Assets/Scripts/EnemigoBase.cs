using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemigoBase : MonoBehaviour
{
    // El moderador de acceso debe ser protected para su uso por el hijo
    protected NavMeshAgent agente;
    protected Transform jugador;

    public float velocidad = 3.5f;

    // --- NUEVO 1: Variables de Daño y Audio ---
    [Header("Ataque")]
    [Tooltip("Pon 1 para normales, 2 para el Jefe")]
    public int danoAtaque = 1;
    public AudioClip sonidoAtaque; // El grito cuando te agarra
    // ------------------------------------------

    // Variable para guardar dónde empezó
    private Vector3 posicionInicial;

    public virtual void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;

        // Guardamos la posición inicial al arrancar
        posicionInicial = transform.position;

        // Buscar automaticamente al jugador
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    public virtual void Update()
    {
        if (jugador != null)
        {
            // Perseguir siempre
            agente.SetDestination(jugador.position);

            // --- NUEVO 2: Detectar si está lo suficientemente cerca para atacar ---
            float distancia = Vector3.Distance(transform.position, jugador.position);

            // Si está pegado a ti (1.2 metros), te ataca
            if (distancia < 1.2f)
            {
                Atacar();
            }
        }
    }

    // Se reutiliza la funcion de atacar para todos los enemigos
    public virtual void Atacar()
    {
        Debug.Log("¡Te atrapó un enemigo!");

        // --- CORRECCIÓN DE ERROR AQUÍ ---
        // Usamos 'jugador.position' en vez de 'Camera.main' para evitar el NullReference
        if (sonidoAtaque != null && jugador != null)
        {
            AudioSource.PlayClipAtPoint(sonidoAtaque, jugador.position, 1.0f);
        }

        // Enviar daño al GameManager
        if (GameManager.instancia != null)
        {
            GameManager.instancia.RecibirDano(danoAtaque);
        }

        // Enfriamiento
        this.enabled = false;
        Invoke("Reactivar", 2.0f);
    }

    // Función auxiliar para volver a activar el script
    void Reactivar()
    {
        this.enabled = true;
    }

    // Para mandarlo a su casa (Tu función original intacta)
    public void ResetearPosicion()
    {
        // Warp es la forma segura de teletransportar un NavMeshAgent
        if (agente != null) agente.Warp(posicionInicial);

        // Aseguramos que el script esté activo al resetear
        this.enabled = true;
    }
}