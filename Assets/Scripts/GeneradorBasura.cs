using UnityEngine;
using UnityEngine.AI;

public class GeneradorBasura : MonoBehaviour
{
    [Header("Configuración")]
    public bool generarAlInicio = true;

    public GameObject[] prefabsBasura;
    public int cantidadAGenerar = 5;
    public Vector3 areaTamano = new Vector3(10, 1, 10);

    [Header("Filtros de Spawn")]
    public float radioBusqueda = 2.0f;
    public float alturaMaximaPermitida = 1.5f;
    public LayerMask capasAEvitar;

    void Start()
    {
        if (generarAlInicio)
        {
            Generar();
        }
    }

    public void Generar()
    {
        int intentosFallidos = 0;
        int i = 0;

        while (i < cantidadAGenerar && intentosFallidos < 100)
        {
            Vector3 puntoAleatorio = transform.position + new Vector3(
                Random.Range(-areaTamano.x / 2, areaTamano.x / 2),
                0,
                Random.Range(-areaTamano.z / 2, areaTamano.z / 2)
            );

            NavMeshHit hit;

            if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioBusqueda, NavMesh.AllAreas))
            {
                bool posicionValida = true;

                if (hit.position.y > transform.position.y + alturaMaximaPermitida) posicionValida = false;
                if (Physics.CheckSphere(hit.position + Vector3.up * 0.5f, 0.3f, capasAEvitar)) posicionValida = false;

                if (posicionValida)
                {
                    Quaternion rotacionAleatoria = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    int indicePrefab = Random.Range(0, prefabsBasura.Length);

                    GameObject basura = Instantiate(prefabsBasura[indicePrefab], hit.position, rotacionAleatoria);
                    basura.transform.SetParent(null);

                    // IMPORTANTE: Asegúrate de que tus prefabs tengan el Tag "Pick"
                    // para que el sistema los encuentre al limpiar.

                    i++;
                }
                else intentosFallidos++;
            }
            else intentosFallidos++;
        }

        if (intentosFallidos >= 100) Debug.LogWarning("El Generador batalló para encontrar lugares válidos.");
    }

    // --- ESTA ES LA FUNCIÓN QUE USARÁ EL GAMEMANAGER AL MORIR ---
    public void ResetearSistema()
    {
        // 1. Buscamos TODA la basura que exista en la escena
        // Como la del futuro no existe, no hay riesgo.
        GameObject[] basuraExistente = GameObject.FindGameObjectsWithTag("Pick");

        foreach (GameObject b in basuraExistente)
        {
            Destroy(b);
        }

        // 2. Generamos nueva basura fresca
        Generar();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = generarAlInicio ? Color.yellow : Color.red;
        Gizmos.DrawWireCube(transform.position, areaTamano);
    }
}