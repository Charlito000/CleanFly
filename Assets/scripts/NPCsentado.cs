using UnityEngine;

public enum NPCSentadoState { Idle, Atacar }

[RequireComponent(typeof(Animator))]
public class NPCsentado : MonoBehaviour
{
    [Header("Detección")]
    public Transform jugador;
    public float rangoAtaque = 3f;

    [Header("Daño")]
    public float dano = 10f;
    public float fuerzaEmpuje = 8f;
    public float fuerzaVertical = 4f;
    public float cooldownGolpe = 1f; // segundos entre golpes

    Animator anim;
    NPCSentadoState state = NPCSentadoState.Idle;
    float timerGolpe = 0f;
    bool puedeGolpear = true;

    static readonly int HashIdle = Animator.StringToHash("Idle");
    static readonly int HashAtaque = Animator.StringToHash("Attack");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float dist = DistanciaHorizontal(transform.position, jugador.position);

        // Cooldown entre golpes
        if (!puedeGolpear)
        {
            timerGolpe -= Time.deltaTime;
            if (timerGolpe <= 0f)
                puedeGolpear = true;
        }

        switch (state)
        {
            case NPCSentadoState.Idle: StateIdle(dist); break;
            case NPCSentadoState.Atacar: StateAtacar(dist); break;
        }
    }

    // ══ Estados ══════════════════════════════════════════════

    void StateIdle(float dist)
    {
        if (dist <= rangoAtaque)
            EnterAtacar();
    }

    void StateAtacar(float dist)
    {
        RotateTowards(jugador.position);

        if (dist > rangoAtaque)
            EnterIdle();
    }

    // ══ Transiciones ═════════════════════════════════════════

    void EnterIdle()
    {
        state = NPCSentadoState.Idle;
        PlayAnim(HashIdle);
    }

    void EnterAtacar()
    {
        state = NPCSentadoState.Atacar;
        PlayAnim(HashAtaque);
    }

    // ══ Colisión con el jugador ═══════════════════════════════

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (state != NPCSentadoState.Atacar) return;
        if (!puedeGolpear) return;

        // Empuja al jugador
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            dir.y = 0;
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * fuerzaEmpuje + Vector3.up * fuerzaVertical,
                        ForceMode.Impulse);
        }

        // Quita vida
        if (HUDManager.instancia != null)
            HUDManager.instancia.RecibirDano(dano);

        // Activa cooldown para no golpear cada frame
        puedeGolpear = false;
        timerGolpe = cooldownGolpe;
    }

    // ══ Helpers ══════════════════════════════════════════════

    void PlayAnim(int hash)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(hash))
            anim.Play(hash);
    }

    float DistanciaHorizontal(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir.magnitude < 0.1f) return;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, rot, Time.deltaTime * 10f);
    }
}
