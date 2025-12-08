using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia; // Singleton para fácil acceso

    [Header("Configuración Nivel Actual")]
    public int basuraObjetivo = 5;
    public GameObject muroBloqueo; // El muro que impide pasar a la siguiente zona

    public int basuraActual = 0;
    public TextMeshProUGUI textoUI; // Arrastra tu texto aquí

    void Awake()
    {
        instancia = this; // Así cualquier script puede decir GameManager.instancia.Recolectar()
    }

    void Start()
    {
        ActualizarUI();
    }

    public void RecolectarBasura()
    {
        basuraActual++;
        ActualizarUI();

        // Checar si ganamos la zona
        if (basuraActual >= basuraObjetivo)
        {
            DesbloquearZona();
        }
    }

    void DesbloquearZona()
    {
        if (muroBloqueo != null)
        {
            Destroy(muroBloqueo); // ¡Adiós muro!

            // Mensaje de feedback
            if (textoUI != null) textoUI.text = "¡ZONA DESBLOQUEADA!\nCORRE...";
            Debug.Log("Muro destruido, avanza a la siguiente zona.");
        }
    }

    void ActualizarUI()
    {
        if (textoUI != null)
            textoUI.text = $"Basura: {basuraActual} / {basuraObjetivo}";
    }
}
