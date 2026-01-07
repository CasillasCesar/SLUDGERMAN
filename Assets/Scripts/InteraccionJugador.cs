using UnityEngine;
using TMPro;            // Necesario para el texto
using UnityEngine.UI;   // Necesario para cambiar el color de la mira

public class InteraccionJugador : MonoBehaviour
{
    [Header("Configuración")]
    public float distanciaAgarre = 3f;
    public Transform camara;
    public AudioClip sonidoRecoger;

    [Header("UI - Feedback Visual")]
    public TextMeshProUGUI textoAyuda;  // El texto "Click para recoger"
    public Image imagenMira;            // Tu imagen de la mira (punto blanco)
    public Color colorNormal = Color.white;
    public Color colorInteraccion = Color.red; // Se pone rojo al mirar basura

    void Update()
    {
        RaycastHit hit;
        // Lanzamos el rayo CONSTANTEMENTE para ver qué miramos
        bool detectoAlgo = Physics.Raycast(camara.position, camara.forward, out hit, distanciaAgarre);

        // --- CORRECCIÓN: Usamos tu tag original "Pick" ---
        bool esObjetoRecogible = detectoAlgo && hit.collider.CompareTag("Pick");

        if (esObjetoRecogible)
        {
            // --- ESTADO: MIRANDO OBJETO ---

            // 1. Mostrar texto
            if (textoAyuda != null) textoAyuda.gameObject.SetActive(true);

            // 2. Cambiar color mira a ROJO
            if (imagenMira != null) imagenMira.color = colorInteraccion;

            // 3. Permitir recoger (Click izquierdo O tecla E)
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
            {
                if (sonidoRecoger != null)
                    AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position, 1.0f);

                Destroy(hit.collider.gameObject);

                if (GameManager.instancia != null)
                {
                    GameManager.instancia.RecolectarBasura();
                }
            }
        }
        else
        {
            // --- ESTADO: NO HAY NADA ---

            // 1. Ocultar texto
            if (textoAyuda != null) textoAyuda.gameObject.SetActive(false);

            // 2. Regresar color mira a BLANCO
            if (imagenMira != null) imagenMira.color = colorNormal;
        }
    }
}