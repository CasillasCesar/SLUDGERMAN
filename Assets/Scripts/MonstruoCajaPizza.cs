using UnityEngine;

public class MonstruoCajaPizza : EnemigoBase
{
    public float distanciaAtaque = 1.5f; // Tiene que acercarse más

    public override void Start()
    {
        velocidad = 7.0f; // ¡Corre mucho más rápido!
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
