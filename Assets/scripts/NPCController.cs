using UnityEngine;
using UnityEngine.AI;

public enum NPCState { Idle, Alert, Chase, Attack, Return }

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    [Header("Detección")]
    public Transform player;
    public float detectRange = 8f;
    public float attackRange = 2f;
    public float loseRange = 12f;

    [Header("Velocidades")]
    public float chaseSpeed = 4f;
    public float returnSpeed = 3f;

    [Header("Tiempos")]
    public float losePlayerTimeout = 3f;
    public float alertDuration = 0.8f;

    Animator anim;
    NavMeshAgent agent;
    NPCState state = NPCState.Idle;
    Vector3 origin;
    float loseTimer;
    float alertTimer;
    [Header("Raqueta")]
    RaquetaCollider raquetaCollider;
    public GameObject raqueta; // arrastra la raqueta aquí en el Inspector

    static readonly int HashIdle = Animator.StringToHash("Idle");
    static readonly int HashWalk = Animator.StringToHash("Walk");
    static readonly int HashAttack = Animator.StringToHash("Attack");
    static readonly int HashAlert = Animator.StringToHash("Alert");

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        origin = transform.position;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange - 0.3f;

        // Busca el componente en la raqueta
        if (raqueta != null)
            raquetaCollider = raqueta.GetComponent<RaquetaCollider>();
    }

    void Update()
    {
        float dist = DistanciaHorizontal(transform.position, player.position);

        switch (state)
        {
            case NPCState.Idle: StateIdle(dist); break;
            case NPCState.Alert: StateAlert(dist); break;
            case NPCState.Chase: StateChase(dist); break;
            case NPCState.Attack: StateAttack(dist); break;
            case NPCState.Return: StateReturn(dist); break;
        }
    }

    // ══ Estados ══════════════════════════════════════════════

    void StateIdle(float dist)
    {
        if (dist <= detectRange)
            EnterAlert();
    }

    void StateAlert(float dist)
    {
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0f)
        {
            if (dist <= attackRange) EnterAttack();
            else EnterChase();
        }
    }

    void StateChase(float dist)
    {
        // Actualiza destino cada frame para seguir al jugador
        agent.SetDestination(player.position);

        if (dist <= attackRange)
        {
            EnterAttack();
            return;
        }

        if (dist > loseRange)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0f)
                EnterReturn();
        }
        else
        {
            loseTimer = losePlayerTimeout;
        }
    }

    void StateAttack(float dist)
    {
        // Quieto, mirando al jugador
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        RotateTowards(player.position);

        // Si el jugador se aleja, perseguir
        if (dist > attackRange + 0.5f)
        {
            if (dist > loseRange)
            {
                loseTimer -= Time.deltaTime;
                if (loseTimer <= 0f)
                    EnterReturn();
            }
            else
            {
                EnterChase();
            }
        }
    }

    void StateReturn(float dist)
    {
        if (dist <= detectRange)
        {
            EnterAlert();
            return;
        }

        // Si el agente ya llegó y se detuvo
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            transform.position = new Vector3(origin.x, transform.position.y, origin.z);
            EnterIdle();
        }
    }

    // ══ Transiciones ═════════════════════════════════════════

    void EnterIdle()
    {
        state = NPCState.Idle;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.updateRotation = true;
        if (raqueta != null) raqueta.SetActive(false);
        PlayAnim(HashIdle);
    }

    void EnterAlert()
    {
        state = NPCState.Alert;
        alertTimer = alertDuration;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        if (raqueta != null) raqueta.SetActive(false);
        PlayAnim(HashAlert);
    }

    void EnterAttack()
    {
        state = NPCState.Attack;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;
        if (raqueta != null) raqueta.SetActive(true);
        if (raquetaCollider != null) raquetaCollider.ActivarGolpe(); // activa el golpe
        PlayAnim(HashAttack);
    }

    void EnterChase()
    {
        state = NPCState.Chase;
        loseTimer = losePlayerTimeout;
        agent.speed = chaseSpeed;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange - 0.3f;
        if (raqueta != null) raqueta.SetActive(false);
        if (raquetaCollider != null) raquetaCollider.DesactivarGolpe(); // desactiva
        PlayAnim(HashWalk);
    }
    void EnterReturn()
    {
        state = NPCState.Return;
        agent.speed = returnSpeed;
        agent.updateRotation = true;
        agent.stoppingDistance = 0f;      // llega exactamente al punto
        agent.autoBraking = true;
        if (raqueta != null) raqueta.SetActive(false);
        agent.SetDestination(origin);     // destino desde el inicio
        PlayAnim(HashWalk);
    }

    // ══ Helpers ══════════════════════════════════════════════

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

    void PlayAnim(int hash)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(hash))
            anim.Play(hash);
    }
}