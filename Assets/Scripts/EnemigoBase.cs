using UnityEngine;
using UnityEngine.AI;

public class EnemigoBase : MonoBehaviour
{
    protected NavMeshAgent agente;
    protected Transform jugador;

    public float velocidad = 3.5f;

    [Header("Ataque")]
    [Tooltip("Pon 1 para normales, 2 para el Jefe")]
    public int danoAtaque = 1;
    public AudioClip sonidoAtaque;

    private Vector3 posicionInicial;

    // --- CAMBIO 1: Variables para suavizar movimiento ---
    private float cronometroRuta = 0f;
    private float tiempoActualizacion = 0.2f; // Se actualiza cada 0.2 segundos
    // --------------------------------------------------

    public virtual void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;
        posicionInicial = transform.position;

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
            // --- CAMBIO 2: Optimización del NavMesh ---
            // Solo le mandamos la orden de moverse si pasó el tiempo
            cronometroRuta += Time.deltaTime;

            if (cronometroRuta >= tiempoActualizacion)
            {
                if (agente.isOnNavMesh && !agente.isStopped)
                {
                    agente.SetDestination(jugador.position);
                }
                cronometroRuta = 0f; // Reiniciamos el contador
            }
            // ------------------------------------------

            // La distancia SÍ se checa en tiempo real para atacar rápido
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (distancia < 1.2f)
            {
                Atacar();
            }
        }
    }

    public virtual void Atacar()
    {
        Debug.Log("¡Te atrapó un enemigo!");

        if (sonidoAtaque != null && jugador != null)
        {
            AudioSource.PlayClipAtPoint(sonidoAtaque, jugador.position, 1.0f);
        }

        if (GameManager.instancia != null)
        {
            GameManager.instancia.RecibirDano(danoAtaque);
        }

        if (agente != null) agente.isStopped = true;
        this.enabled = false;

        Invoke("Reactivar", 2.0f);
    }

    void Reactivar()
    {
        this.enabled = true;
        if (agente != null) agente.isStopped = false;
    }

    public void ResetearPosicion()
    {
        if (agente != null) agente.Warp(posicionInicial);
        this.enabled = true;
    }
}