using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class ControlMenu : MonoBehaviour
{
    void Start()
    {
        // IMPORTANTE: Aseguramos que el mouse se vea y se pueda mover
        // (Por si vienes de la escena de juego donde estaba bloqueado)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Jugar()
    {
        // Carga la escena número 1 de la lista de Build Settings
        SceneManager.LoadScene(1);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}