using UnityEngine;

public class InteraccionJugador : MonoBehaviour
{
    public float distanciaAgarre = 3f;
    public Transform camara; // Asigna tu MainCamera aquí

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(camara.position, camara.forward, out hit, distanciaAgarre))
            {
                if (hit.collider.CompareTag("Pick"))
                {
                    // 1. Destruir la basura visual
                    Destroy(hit.collider.gameObject);

                    // 2. Avisar al GameManager
                    if (GameManager.instancia != null)
                    {
                        GameManager.instancia.RecolectarBasura();
                    }
                }
            }
        }
    }
}
