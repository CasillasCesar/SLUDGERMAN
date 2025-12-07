using UnityEngine;
using UnityEngine.AI;   // No lo olvides, no seas tonoto

public class EnemigoBase : MonoBehaviour
{
    // El moderador de acceso debe ser protected para su uso por el hijo
    protected NavMeshAgent agente;
    protected Transform jugador; // Referencia al Jugador

    public float velocidad = 3.5f;
    // "virtual" para poder hacer override al metodo
    public virtual void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidad;

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
}
