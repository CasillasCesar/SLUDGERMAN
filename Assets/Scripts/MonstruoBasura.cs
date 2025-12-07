using UnityEngine;

// Heredamos de la clase Enemigobase
public class MonstruoBasura : EnemigoBase
{
    public float distanciaAtaque = 2.0f;

    // Sobreescribimos el Update del padre para agregar el ataque
    public override void Update()
    {
        base.Update(); // Ejecuta el scripts dle padre primero

        // Lógica exclusiva de este monstruo
        if (jugador != null)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (distancia < distanciaAtaque)
            {
                Atacar(); // Usa una funcion heredada de padre
            }
        }
    }
}