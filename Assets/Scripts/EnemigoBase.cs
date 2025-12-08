using UnityEngine;
using UnityEngine.AI;   // No lo olvides, no seas tonoto
using UnityEngine.SceneManagement;

public class EnemigoBase : MonoBehaviour
{
    // El moderador de acceso debe ser protected para su uso por el hijo
    protected NavMeshAgent agente;
    protected Transform jugador; // Referencia al Jugador

    public float velocidad = 3.5f;

    // --- NUEVO: Variable para guardar dónde empezó ---
    private Vector3 posicionInicial;

    // "virtual" para poder hacer override al metodo
    public virtual void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;

        // --- NUEVO: Guardamos la posición inicial al arrancar ---
        posicionInicial = transform.position;

        // Buscar automaticamente al jugador
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if(objJugador != null)
        {
            jugador = objJugador.transform;
        }
    }

    public virtual void Update()
    {
        // Lógica básica para perseguir siempre
        if (jugador != null)
        {
            agente.SetDestination(jugador.position);
        }
    }

    // Se reutiliza la funcion de atacar para todos los enemigos
    public virtual void Atacar()
    {
        Debug.Log("¡Te atrapó un enemigo!");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (GameManager.instancia != null)
        {
            GameManager.instancia.RespawnJugador();
        }
    }

    // --- NUEVA FUNCIÓN: Para mandarlo a su casa ---
    public void ResetearPosicion()
    {
        // Warp es la forma segura de teletransportar un NavMeshAgent
        agente.Warp(posicionInicial);
    }
}
