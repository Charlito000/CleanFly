using UnityEngine;

public class FlyMovement : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadBase = 8f;
    public float velocidadTurbo = 18f;
    public float sensibilidadRaton = 2f;

    [Header("Suavizado")]
    public float suavizadoAceleracion = 20f;

    private Rigidbody rb;
    private float rotacionX = 0f;
    private float rotacionY = 0f;
    private Vector3 velocidadActual;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Solo guardamos los valores del mouse, no aplicamos rotacion aqui
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton;
        rotacionY += mouseX;
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -80f, 80f);
    }

    void FixedUpdate()
    {
        ManejarRotacion();
        ManejarMovimiento();
    }

    void ManejarRotacion()
    {
        // Rotacion aplicada por el Rigidbody, no por transform
        Quaternion rotObjetivo = Quaternion.Euler(rotacionX, rotacionY, 0f);
        rb.MoveRotation(rotObjetivo);
    }

    void ManejarMovimiento()
    {
        bool turbo = Input.GetKey(KeyCode.LeftShift);
        float velocidad = turbo ? velocidadTurbo : velocidadBase;

        float avance = Input.GetAxis("Vertical");

        float vertical = 0f;
        if (Input.GetKey(KeyCode.Space)) vertical = 1f;
        if (Input.GetKey(KeyCode.LeftControl)) vertical = -1f;

        Vector3 direccion = transform.forward * avance;
        direccion += Vector3.up * vertical;

        rb.velocity = direccion * velocidad;
    }
}