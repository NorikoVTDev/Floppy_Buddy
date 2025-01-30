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
    [SerializeField] private float guardRange = 5f;
    [SerializeField] private float runAwayThreshold = 30f;
    [SerializeField] private float guardHealthThreshold = 50f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float ragdollCooldown = 3f;

    // Internal State
    private int health = 100;
    private bool isAttacking = false;
    private bool isGuarding = false;
    private bool isRunningAway = false;
    private bool isRagdoll = false;

    private Transform player;
    public GameObject playerRagdollRoot;
    public Transform playerRb;
    private EnemySpawner spawner;

    private bool alive = true;

    public float minCollisionForce = 10f; // Minimum force required to trigger detection

    [SerializeField] private Transform EnemyRagdollRoot;
    [SerializeField] private bool StartRagdoll = false;
    public Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;
    private Collider[] Colliders;

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

    // Public method to set the player reference
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

    // Public method to set the spawner reference
    public void SetSpawner(EnemySpawner spawnerReference)
    {
        spawner = spawnerReference;
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

        // Disable ragdoll physics at the start
        //EnableRagdoll(false);
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
        //else if (health <= runAwayThreshold && !isRunningAway)
        //{
        //    RunAway();
        //}
        //else if (health <= guardHealthThreshold && !isGuarding)
        //{
        //    Guard();
        //}
        if (distanceToPlayer <= attackRange)
        {
            Attack(distanceToPlayer);
        }
        if (distanceToPlayer > attackRange && !isRunningAway && !isGuarding)
        {
            MoveTowardPlayer();
            // Update player location every frame
            // Check if agent is active
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
        // Stop moving and attack the player
        //agent.isStopped = true;
        isAttacking = true;

        // Perform attack logic
        //if (distanceToPlayer <= attackRange)
        //{
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy attacked player for {attackDamage} damage!");
            }
        //}

        // Reset attack state after a delay
        Invoke("ResetAttack", 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
        //agent.isStopped = false;
    }

    private void Guard()
    {
        // Stop moving and guard
        agent.isStopped = true;
        isGuarding = true;

        // Perform guard logic
        Debug.Log("Guarding!");

        // Reset guard state after a delay
        Invoke("ResetGuard", 2f);
    }

    private void ResetGuard()
    {
        isGuarding = false;
        agent.isStopped = false;
    }

    private void RunAway()
    {
        // Move away from the player
        Vector3 direction = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + direction * 10f);
        isRunningAway = true;
    }

    private void UpdateAnimations()
    {
        // Update Animator parameters based on the current state
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f && !isAttacking && !isGuarding && !isRunningAway);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isGuarding", isGuarding);
        animator.SetBool("isRunningAway", isRunningAway);
    }

    public void TakeDamage(int damage, Vector3 forceDirection, float forceStrength)
    {
        if (isRagdoll) return;

        // Reduce health and check for death
        health -= damage;
        if (health <= 0)
        {
            //Die();
        }
        else
        {
            //GoRagdoll();
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
        // Apply force to all Rigidbody components
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            Debug.Log("POW!");
            rb.AddForce(direction * strength, ForceMode.Impulse);
        }
    }

    private Vector3 GetGroundPosition()
    {
        // Find the ground position below the enemy
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            return hit.point;
        }
        return transform.position;
    }

    private void Die(Vector3 collisionDirection)
    {
        // Disable AI and animator
        agent.enabled = false;
        animator.enabled = false;
        alive = false;

        // Enable ragdoll physics permanently
        EnableRagdoll();

        // Add a force in the direction of the collision
        Vector3 forceDirection = collisionDirection.normalized;
        //Doesn't seem to work
        ApplyRagdollForce(forceDirection, 1000f);
        //EnableRagdoll(true);

        // Notify the spawner that this enemy has been killed
        if (spawner != null)
        {
            spawner.EnemyKilled();
        }

        // Destroy the enemy object after a delay
        //Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter(Collider collider)
    {
        // Check if the colliding object is the specific ragdoll or part of it
        if (collider.gameObject == playerRagdollRoot)
        {
            // Get the Rigidbody of the colliding object
            Rigidbody ragdollRigidbody = collider.GetComponent<Rigidbody>();

            if (ragdollRigidbody != null)
            {
                // Calculate the collision force (magnitude of velocity)
                float collisionForce = ragdollRigidbody.linearVelocity.magnitude;

                // Check if the force is above the threshold
                if (collisionForce >= minCollisionForce)
                {
                    // Get the direction of the collision
                    Vector3 collisionDirection = ragdollRigidbody.transform.position - collider.transform.position;
                    Debug.Log("Specific ragdoll hit the enemy with enough force: " + collisionForce);
                    Die(collisionDirection);
                }
            }
        }
    }
}