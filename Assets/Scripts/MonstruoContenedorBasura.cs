using UnityEngine;

public class MonstruoContenedorBasura : EnemigoBase
{
    public float distanciaAtaque = 3.5f; // Te mata desde lejos porque es enorme

    public override void Start()
    {
        velocidad = 2.5f; // Lento y pesado
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (jugador != null && Vector3.Distance(transform.position, jugador.position) < distanciaAtaque)
        {
            Atacar();
        }
    }
}
