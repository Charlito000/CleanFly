using UnityEngine;

public class RaquetaCollider : MonoBehaviour
{
    [Header("Empuje")]
    public float fuerzaEmpuje = 10f;
    public float fuerzaVertical = 5f;  // cuánto sube el jugador al ser golpeado

    [Header("Daño (opcional)")]
    public float dano = 10f;

    bool puedeGolpear = false; // solo activo durante la animación de ataque

    // El NPCController llama a estos métodos
    public void ActivarGolpe() => puedeGolpear = true;
    public void DesactivarGolpe() => puedeGolpear = false;

    void OnTriggerEnter(Collider other)
    {
        if (!puedeGolpear) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            dir.y = 0;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * fuerzaEmpuje + Vector3.up * fuerzaVertical,
                        ForceMode.Impulse);
        }

        // Quita vida al jugador
        HUDManager.instancia.RecibirDano(dano);

        puedeGolpear = false;
    }
}
