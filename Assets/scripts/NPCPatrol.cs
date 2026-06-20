using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    [Header("Puntos de patrulla")]
    public Transform[] waypoints;
    public float waitTime = 5f;
    public float arrivalThreshold = 0.5f; // distancia para considerar "llegó"

    private NavMeshAgent agent;
    private Animator anim;
    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (waypoints.Length > 0)
            GoToNextWaypoint();
    }

    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (anim != null)
            anim.SetFloat("speed", agent.velocity.magnitude);

        if (agent.pathPending) return;

        // ✅ Condición robusta: solo usa remainingDistance
        bool arrived = agent.remainingDistance <= arrivalThreshold;

        if (arrived)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
                agent.ResetPath(); // evita que siga recalculando
            }

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                GoToNextWaypoint();
            }
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }
}