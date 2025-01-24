using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // References
    public EnemySpawner spawner;
    public Transform player;

    // AI Settings
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float guardRange = 5f;
    public float runAwayThreshold = 30f;
    public float guardHealthThreshold = 50f;
    public float jumpCooldown = 2f;
    public float ragdollCooldown = 3f;
    public int attackDamage = 10;
    public bool isRagdoll = false;
    public Rigidbody playerRb;
    public GameObject swipeHimObject;

    // Internal State
    private NavMeshAgent agent;
    private Animator animator;
    private int health = 100;
    private bool isAttacking = false;
    private bool isGuarding = false;
    private bool isRunningAway = false;
    private PlayerHealth playerHealth;
    private RagdollHelper ragdollHelper;

    private void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on player!");
        }

        // Get components
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Configure NavMeshAgent
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;

        animator.applyRootMotion = false;

        // Access the playerRb reference from the swipeHim singleton
        if (SwipeHim.Instance != null)
        {
            playerRb = SwipeHim.Instance.playerRb;
        }
        else
        {
            Debug.LogError("swipeHim singleton instance not found!");
        }

        // Create body parts if they don't exist
        CreateBodyParts();

        // Add RagdollHelper component dynamically
        ragdollHelper = gameObject.AddComponent<RagdollHelper>();

        // Configure body parts (you can do this programmatically or via inspector if possible)
        ragdollHelper.bodyParts = new RagdollHelper.BodyPart[]
        {
            new RagdollHelper.BodyPart { transform = transform.Find("Head"), colliderRadius = 0.1f, isSpherical = true },
            new RagdollHelper.BodyPart { transform = transform.Find("Torso"), colliderSize = new Vector3(0.5f, 1f, 0.3f) },
            new RagdollHelper.BodyPart { transform = transform.Find("Arm_L"), colliderSize = new Vector3(0.2f, 0.7f, 0.2f) },
            new RagdollHelper.BodyPart { transform = transform.Find("Arm_R"), colliderSize = new Vector3(0.2f, 0.7f, 0.2f) },
            new RagdollHelper.BodyPart { transform = transform.Find("Leg_L"), colliderSize = new Vector3(0.3f, 0.9f, 0.3f) },
            new RagdollHelper.BodyPart { transform = transform.Find("Leg_R"), colliderSize = new Vector3(0.3f, 0.9f, 0.3f) }
        };

        // Disable ragdoll physics at the start
        ragdollHelper.EnableRagdoll(false);
    }

    private void CreateBodyParts()
    {
        // Create the head if it doesn't exist
        if (transform.Find("Head") == null)
        {
            GameObject head = new GameObject("Head");
            head.transform.SetParent(transform);
            head.transform.localPosition = new Vector3(0, 1.5f, 0); // Adjust position as needed
        }

        // Create the torso if it doesn't exist
        if (transform.Find("Torso") == null)
        {
            GameObject torso = new GameObject("Torso");
            torso.transform.SetParent(transform);
            torso.transform.localPosition = new Vector3(0, 1f, 0); // Adjust position as needed
        }

        // Create the left arm if it doesn't exist
        if (transform.Find("Arm_L") == null)
        {
            GameObject armL = new GameObject("Arm_L");
            armL.transform.SetParent(transform);
            armL.transform.localPosition = new Vector3(-0.5f, 1f, 0); // Adjust position as needed
        }

        // Create the right arm if it doesn't exist
        if (transform.Find("Arm_R") == null)
        {
            GameObject armR = new GameObject("Arm_R");
            armR.transform.SetParent(transform);
            armR.transform.localPosition = new Vector3(0.5f, 1f, 0); // Adjust position as needed
        }

        // Create the left leg if it doesn't exist
        if (transform.Find("Leg_L") == null)
        {
            GameObject legL = new GameObject("Leg_L");
            legL.transform.SetParent(transform);
            legL.transform.localPosition = new Vector3(-0.25f, 0f, 0); // Adjust position as needed
        }

        // Create the right leg if it doesn't exist
        if (transform.Find("Leg_R") == null)
        {
            GameObject legR = new GameObject("Leg_R");
            legR.transform.SetParent(transform);
            legR.transform.localPosition = new Vector3(0.25f, 0f, 0); // Adjust position as needed
        }
    }

    private void Update()
    {
        // Ensure the player reference is valid
        if (playerRb == null)
        {
            Debug.LogError("Player reference is missing!");
            return;
        }

        // Update player location every frame
        agent.SetDestination(playerRb.position);

        // AI Logic
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (health <= runAwayThreshold && !isRunningAway)
        {
            RunAway();
        }
        else if (health <= guardHealthThreshold && !isGuarding)
        {
            Guard();
        }
        else if (distanceToPlayer <= attackRange && !isAttacking)
        {
            Attack(distanceToPlayer);
        }
        else if (distanceToPlayer > attackRange && !isRunningAway && !isGuarding)
        {
            MoveTowardPlayer();
        }

        // Update Animations
        UpdateAnimations();
    }

    void MoveTowardPlayer()
    {

    }

    private void Attack(float distanceToPlayer)
    {
        // Stop moving and attack the player
        agent.isStopped = true;
        isAttacking = true;

        // Perform attack logic
        if (playerHealth != null && distanceToPlayer <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked player for {attackDamage} damage!");
        }

        // Reset attack state after a delay
        Invoke("ResetAttack", 1f);
    }

    void ResetAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
    }

    void Guard()
    {
        // Stop moving and guard
        agent.isStopped = true;
        isGuarding = true;

        // Perform guard logic
        Debug.Log("Guarding!");

        // Reset guard state after a delay
        Invoke("ResetGuard", 2f);
    }

    void ResetGuard()
    {
        isGuarding = false;
        agent.isStopped = false;
    }

    void RunAway()
    {
        // Move away from the player
        Vector3 direction = (transform.position - player.position).normalized;
        agent.SetDestination(transform.position + direction * 10f);
        isRunningAway = true;
    }

    void UpdateAnimations()
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
            Die();
        }
        else
        {
            GoRagdoll();
            ApplyRagdollForce(forceDirection, forceStrength);
        }
    }

    private void GoRagdoll()
    {
        isRagdoll = true;

        // Disable AI and animator
        agent.enabled = false;
        animator.enabled = false;

        // Enable ragdoll physics
        ragdollHelper.EnableRagdoll(true);

        // Start cooldown to get back up
        Invoke("GetUp", ragdollCooldown);
    }

    private void ApplyRagdollForce(Vector3 direction, float strength)
    {
        foreach (Rigidbody rb in ragdollHelper.GetRagdollRigidbodies())
        {
            rb.AddForce(direction * strength, ForceMode.Impulse);
        }
    }

    private void GetUp()
    {
        isRagdoll = false;

        // Disable ragdoll physics
        ragdollHelper.EnableRagdoll(false);

        // Re-enable AI and animator
        animator.enabled = true;
        agent.enabled = true;

        // Reset the NavMeshAgent's destination
        agent.isStopped = false;
        agent.SetDestination(playerRb.position);

        // Reset the enemy's position and rotation
        transform.position = GetGroundPosition();
        transform.rotation = Quaternion.identity;
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

    void Die()
    {
        // Disable AI and animator
        agent.enabled = false;
        animator.enabled = false;

        // Enable ragdoll physics permanently
        ragdollHelper.EnableRagdoll(true);
        
        // Notify the spawner that this enemy has been killed
        if (spawner != null)
        {
            spawner.EnemyKilled();
        }

        // Destroy the enemy object after a delay
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isRagdoll) return;

        // Calculate collision force
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        Vector3 forceDirection = collision.contacts[0].normal;

        // Apply damage and force to the ragdoll
        TakeDamage(10, -forceDirection, collisionForce * 0.1f);
    }
}
