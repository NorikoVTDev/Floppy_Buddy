using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // References (assign in Inspector or programmatically)
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    // AI Settings (assign in Inspector)
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float deathForce = 2f;
    public SoundController soundController;

    // Internal State
    private int health = 100;
    private bool isAttacking = false;
    private bool isGuarding = false;
    private bool isRunningAway = false;
    private bool isRagdoll = false;

    private Transform player;
    public GameObject playerRagdollRoot;
    private EnemySpawner spawner;

    private bool alive = true;

    public float minCollisionForce = 10f; // Minimum force required to trigger detection

    [SerializeField] private Transform EnemyRagdollRoot;
    [SerializeField] private bool StartRagdoll = false;
    public Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;
    private Collider[] Colliders;

    // Reference to the WavesController
    public WavesController wavesController;

    private void Awake()
    {
        Rigidbodies = EnemyRagdollRoot.GetComponentsInChildren<Rigidbody>();
        Joints = EnemyRagdollRoot.GetComponentsInChildren<CharacterJoint>();
        Colliders = EnemyRagdollRoot.GetComponentsInChildren<Collider>();

        if (StartRagdoll)
        {
            EnableRagdoll();
        }
    }

    public void SetPlayer(Transform playerTransform, GameObject playerRagdollRoot)
    {
        player = playerTransform;
        this.playerRagdollRoot = playerRagdollRoot;
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned!");
        }
        else
        {
            Debug.Log($"Player reference assigned: {player.name}");
        }
    }

    public void SetSpawner(EnemySpawner spawnerReference)
    {
        spawner = spawnerReference;
    }

    public void setSoundController(SoundController soundControllerReference)
    {
        soundController = soundControllerReference;
    }

    private void Start()
    {
        // Access the player reference from the swipeHim singleton
        if (SwipeHim.Instance != null)
        {
            player = SwipeHim.Instance.playerRb.transform;
            Debug.Log($"Player reference found: {player != null}");
        }
        else
        {
            Debug.LogError("swipeHim singleton instance not found!");
        }

        // Ensure references are assigned
        if (agent == null) Debug.LogError("NavMeshAgent is not assigned!");
        if (animator == null) Debug.LogError("Animator is not assigned!");

        // Configure NavMeshAgent
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;

        // If WavesController is not assigned, try to find it automatically
        if (wavesController == null)
        {
            wavesController = FindFirstObjectByType<WavesController>();
        }
    }

    private void Update()
    {
        // Ensure references are valid
        if (agent == null || animator == null || player == null)
        {
            Debug.LogError("Required references are missing!");
            return;
        }

        // AI Logic
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (!alive)
        {
            return;
        }
        if (distanceToPlayer <= attackRange)
        {
            Attack(distanceToPlayer);
        }
        if (distanceToPlayer > attackRange && !isRunningAway && !isGuarding)
        {
            MoveTowardPlayer();
            if (agent.enabled)
            {
                agent.SetDestination(player.position);
            }
        }

        // Update Animations
        UpdateAnimations();
    }

    private void MoveTowardPlayer()
    {
        // Move toward the player
    }

    private void Attack(float distanceToPlayer)
    {
        soundController.GetRandomSound("Sound/attack","*.wav");
        isAttacking = true;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked player for {attackDamage} damage!");
        }

        Invoke("ResetAttack", 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void Guard()
    {
        agent.isStopped = true;
        isGuarding = true;
        Debug.Log("Guarding!");
        Invoke("ResetGuard", 2f);
    }

    private void ResetGuard()
    {
        isGuarding = false;
        agent.isStopped = false;
    }

    private void RunAway()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + direction * 10f);
        isRunningAway = true;
    }

    private void UpdateAnimations()
    {
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking && !isGuarding && !isRunningAway);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isGuarding", isGuarding);
        animator.SetBool("isRunningAway", isRunningAway);
    }

    public void TakeDamage(int damage, Vector3 forceDirection, float forceStrength)
    {
        if (isRagdoll) return;

        health -= damage;
        if (health <= 0)
        {
            Die(forceDirection);
        }
        else
        {
            ApplyRagdollForce(forceDirection, forceStrength);
        }
    }

    private void EnableRagdoll()
    {
        isRagdoll = true;
        agent.enabled = false;
        animator.enabled = false;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = true;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
        }
    }

    private void ApplyRagdollForce(Vector3 direction, float strength)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            Debug.Log("POW! in direction " + direction);
            rb.AddForce(direction * strength * deathForce, ForceMode.Impulse);
        }
    }

    private void Die(Vector3 collisionDirection)
    {
        soundController.GetRandomSound("Sound/thud","*.mp3");
        soundController.GetRandomSound("Sound/death","*.wav");
        agent.enabled = false;
        animator.enabled = false;
        alive = false;

        EnableRagdoll();

        ApplyRagdollForce(collisionDirection, 2f);

        if (spawner != null)
        {
            spawner.EnemyKilled();
        }

        // Notify the WavesController that an enemy was killed
        if (wavesController != null)
        {
            wavesController.EnemyKilled();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Check if the collider is a child of playerRagdollRoot
        if (collider.transform.IsChildOf(playerRagdollRoot.transform))
        {
            Rigidbody ragdollRigidbody = collider.GetComponent<Rigidbody>();
            if (ragdollRigidbody != null)
            {
                float collisionForce = ragdollRigidbody.linearVelocity.magnitude;
                if (collisionForce >= minCollisionForce)
                {
                    // Calculate the collision direction based on the child's position
                    Vector3 collisionDirection = collider.transform.position - transform.position;
                    Debug.Log("A child of the ragdoll hit the enemy with enough force: " + collisionForce);
                    Die(collisionDirection);
                }
            }
        }
}
}