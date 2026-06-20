using UnityEngine;

public class FlyCamara : MonoBehaviour
{
    [Header("Referencia al jugador")]
    public Transform jugador;

    [Header("Posición")]
    public float distanciaDetras = 4f;
    public float alturaArriba = 1.5f;

    [Header("Suavizado")]
    public float suavizado = 12f;        // qué tan rápido sigue al jugador
    public float suavizadoRotacion = 8f; // qué tan rápido gira hacia el jugador

    private Vector3 posicionFiltrada;

    void Start()
    {
        if (jugador == null) return;
        posicionFiltrada = jugador.position
            - jugador.forward * distanciaDetras
            + Vector3.up * alturaArriba;
        transform.position = posicionFiltrada;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 posObjetivo = jugador.position
            - jugador.forward * distanciaDetras
            + Vector3.up * alturaArriba;

        // Lerp para seguir al jugador rápido
        posicionFiltrada = Vector3.Lerp(
            posicionFiltrada,
            posObjetivo,
            suavizado * Time.deltaTime
        );

        transform.position = posicionFiltrada;

        // Rotación suavizada por separado para evitar temblor al mirar
        Quaternion rotObjetivo = Quaternion.LookRotation(
            (jugador.position + jugador.up * 0.5f) - transform.position
        );
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotObjetivo,
            suavizadoRotacion * Time.deltaTime
        );
    }
}