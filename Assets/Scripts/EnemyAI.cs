using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Layer Masks")]
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("Stats")]
    public float health = 120f;

    [Header("Patrolling")]
    public float walkPointRange = 5f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attacking")]
    public float timeBetweenAttacks = 0.5f;
    private bool alreadyAttacked;
    public GameObject projectile;

    [Header("Rotation")]
    public float turnSpeed = 12f; // higher = faster turning

    [Header("Ranges")]
    public float sightRange = 20f;
    public float attackRange = 12f;
    public bool playerInSightRange;
    public bool playerInAttackRange;


    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }
    }
    private void Awake()
    {
        // Auto-find refs if not assigned
        if (player == null)
        {
            GameObject p = GameObject.Find("PlayerObj");
            if (p != null) player = p.transform;
        }

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patrolling()
    {
        // IMPORTANT: if we previously attacked, the agent was stopped.
        agent.isStopped = false;
        agent.updateRotation = true;

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        if (walkPointSet && Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        // Cast from ABOVE so it reliably hits the ground
        Vector3 rayStart = new Vector3(
            transform.position.x + randomX,
            transform.position.y + 5f,
            transform.position.z + randomZ
        );

        // Raycast down to find the ground point
        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, whatIsGround))
        {
            // Make sure the chosen point is actually on the NavMesh
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                walkPoint = navHit.position;
                walkPointSet = true;
            }
        }
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.updateRotation = true;   // agent controls turning while chasing
        agent.SetDestination(player.position);
    }

    private Vector3 GetAimPoint()
    {
        // Aim at the player's chest instead of feet
        return player.position + Vector3.up * 1.0f;
    }

    private void AttackPlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            ChasePlayer();
            return;
        }

        // Stop moving and stop navmesh from rotating us
        agent.isStopped = true;
        agent.updateRotation = false;
        agent.velocity = Vector3.zero;

        // Rotate toward player (Y only)
        Vector3 dir = GetAimPoint() - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                turnSpeed * 100f * Time.deltaTime
            );
        }

        // Only shoot when accurately facing the player
        float angle = Vector3.Angle(transform.forward, dir.normalized);

        if (!alreadyAttacked && angle < 3f)
        {
            if (projectile != null)
            {
                Vector3 spawnPos = transform.position + transform.forward * 1f + Vector3.up * 1f;
                GameObject projObj = Instantiate(projectile, spawnPos, Quaternion.identity);

                Rigidbody rb = projObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                    rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0f)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}