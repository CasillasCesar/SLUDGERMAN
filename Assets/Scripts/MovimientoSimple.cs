using UnityEngine;

public class MovimientoSimple : MonoBehaviour
{
    public float velocidad = 5f;
    public float sensibilidadMouse = 2f;

    private Rigidbody rb;
    private Transform camara;
    private float rotacionX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camara = transform.GetChild(0); // Busca la c�mara hijo
        Cursor.lockState = CursorLockMode.Locked; // Oculta el mouse
    }

    void Update()
    {
        // Vista (Mouse)
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movimiento (Teclado)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 mover = transform.right * x + transform.forward * z;
        // Moverse sin volar
        Vector3 velocidadFinal = mover * velocidad;
        velocidadFinal.y = rb.linearVelocity.y; // Mantener gravedad

        rb.linearVelocity = velocidadFinal;
    }
}