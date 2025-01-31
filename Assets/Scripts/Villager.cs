using UnityEngine;

public class Villager : MonoBehaviour
{
    [Header("Settings")]
    public float fleeDistance = 5f; // Distance at which the NPC starts fleeing
    public float fleeSpeed = 3f; // Speed at which the NPC runs away
    public float detachProbability = 0.5f; // Chance for a body part to detach
    public GameObject bloodEffect; // Blood effect to spawn when a body part detaches

    private Animator animator;
    private Transform player;
    private bool isFleeing = false;
    private bool isRagdoll = false;

    // Reference to the WavesController
    public WavesController wavesController; // Assign this in the Inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Start in idle state (waving for help)
        animator.SetBool("IsWaving", true);

        // If WavesController is not assigned, try to find it automatically
        if (wavesController == null)
        {
            wavesController = FindFirstObjectByType<WavesController>();
        }
    }

    void Update()
    {
        if (isRagdoll) return; // Don't do anything if in ragdoll state

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fleeDistance && !isFleeing)
        {
            // Start fleeing if player is too close
            StartFleeing();
        }
        else if (distanceToPlayer >= fleeDistance && isFleeing)
        {
            // Stop fleeing if player is far enough away
            StopFleeing();
        }

        if (isFleeing)
        {
            // Move away from the player
            Vector3 direction = (transform.position - player.position).normalized;
            transform.position += direction * fleeSpeed * Time.deltaTime;
        }
    }

    void StartFleeing()
    {
        isFleeing = true;
        animator.SetBool("IsWaving", false);
        animator.SetBool("IsRunning", true);
    }

    void StopFleeing()
    {
        isFleeing = false;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWaving", true);
    }

    public void Hit()
    {
        // Trigger ragdoll effect
        isRagdoll = true;
        animator.enabled = false; // Disable animator to enable ragdoll physics
        EnableRagdoll();

        // Check if the player is on wave 5 or higher
        if (wavesController != null && wavesController.currentWave >= 5)
        {
            // Chance to detach a body part and spawn blood
            if (Random.value < detachProbability)
            {
                DetachBodyPart();
            }
        }
    }

    void EnableRagdoll()
    {
        // Enable all rigidbodies and colliders in the ragdoll hierarchy
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }

    void DetachBodyPart()
    {
        // Get all body parts that can be detached (e.g., limbs with Rigidbody and Collider)
        Rigidbody[] bodyParts = GetComponentsInChildren<Rigidbody>();

        if (bodyParts.Length > 0)
        {
            // Randomly select a body part to detach
            int index = Random.Range(0, bodyParts.Length);
            Rigidbody selectedPart = bodyParts[index];

            // Detach the body part by enabling its rigidbody and removing its parent
            selectedPart.transform.SetParent(null);
            selectedPart.isKinematic = false;

            // Add force to make it "pop off"
            selectedPart.AddForce(Random.onUnitSphere * 10f, ForceMode.Impulse);

            // Spawn blood effect at the detachment point
            if (bloodEffect != null)
            {
                Instantiate(bloodEffect, selectedPart.position, Quaternion.identity);
            }
        }
    }
}