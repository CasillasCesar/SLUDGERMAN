using UnityEngine;
using UnityEngine.AI;

public class GeneradorBasura : MonoBehaviour
{
    public GameObject[] prefabsBasura;
    public int cantidadAGenerar = 10;
    public Vector3 areaTamano = new Vector3(10, 1, 10);

    [Header("Filtros de Spawn")]
    public float radioBusqueda = 2.0f;
    public float alturaMaximaPermitida = 1.5f; // <--- NUEVO: Si Y es mayor a esto, no spawnea
    public LayerMask capasAEvitar; // <--- NUEVO: Capas que bloquean el spawn (Paredes, Cajas)

    void Start()
    {
        Generar();
    }

    void Generar()
    {
        int intentosFallidos = 0; // Seguridad para no congelar Unity
        int i = 0;

        while (i < cantidadAGenerar && intentosFallidos < 100)
        {
            Vector3 puntoAleatorio = transform.position + new Vector3(
                Random.Range(-areaTamano.x / 2, areaTamano.x / 2),
                0,
                Random.Range(-areaTamano.z / 2, areaTamano.z / 2)
            );

            NavMeshHit hit;

            // 1. Preguntamos al NavMesh
            if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioBusqueda, NavMesh.AllAreas))
            {
                bool posicionValida = true;

                // --- FILTRO 1: ALTURA (Evitar Techos) ---
                if (hit.position.y > transform.position.y + alturaMaximaPermitida)
                {
                    posicionValida = false;
                }

                // --- FILTRO 2: ESPACIO LIBRE (Evitar aparecer dentro de cosas) ---
                if (Physics.CheckSphere(hit.position + Vector3.up * 0.5f, 0.3f, capasAEvitar))
                {
                    posicionValida = false;
                }

                // --- RESULTADO ---
                if (posicionValida)
                {
                    Quaternion rotacionAleatoria = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    int indicePrefab = Random.Range(0, prefabsBasura.Length);
                    // Al quitar 'transform', la basura se crea suelta en la escena y NO se deforma
                    GameObject basura = Instantiate(prefabsBasura[indicePrefab], hit.position, rotacionAleatoria);

                    basura.transform.SetParent(null);
                    i++; // Éxito, pasamos a la siguiente basura
                }
                else
                {
                    intentosFallidos++; // Falló el filtro, intentamos de nuevo
                }
            }
            else
            {
                intentosFallidos++; // No encontró NavMesh
            }
        }

        if (intentosFallidos >= 100) Debug.LogWarning("El Generador batalló para encontrar lugares válidos.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaTamano);
    }
}
