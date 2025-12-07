using UnityEngine;

public class MonstruoBotella : EnemigoBase
{
    public float distanciaAtaque = 2.0f;

    public override void Start()
    {
        velocidad = 4.5f; // Velocidad normal
        base.Start();     // Vital: Llama al Start del padre para que configure el NavMesh
    }

    public override void Update()
    {
        base.Update();

        // Lógica exclusiva de este monstruo
        if (jugador != null)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (jugador != null && Vector3.Distance(transform.position, jugador.position) < distanciaAtaque)
            {
                Atacar(); 
            }
        }
    }
}
