using UnityEngine;

public class SwipeHim : MonoBehaviour
{
    public enum ForceModeSelector
    {
        Force,
        Impulse,
        VelocityChange,
        Acceleration
    }

    public Camera mainCamera;
    public float forceMultiplier = 2f;
    public float verticalAngle = -0.2f;
    public float minSwipeDistance = 50f;
    public bool applyToAllParts = true;
    public bool addRandomForce = true;
    public float randomForceMultiplier = 3f;
    public ForceModeSelector forceMode = ForceModeSelector.Impulse;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private bool isSwiping = false;

    public Rigidbody playerRb; // Reference to the Rigidbody component

    // Minimum force required to trigger the "pop off" event
    public float minForceForPopOff = 10f;

    // Singleton instance
    public static SwipeHim Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        DetectSwipe();
    }

    void DetectSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosition = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            swipeEndPosition = Input.mousePosition;
            isSwiping = false;
            Vector2 swipeDelta = swipeEndPosition - swipeStartPosition;
            float swipeDistance = swipeDelta.magnitude;

            if (swipeDistance >= minSwipeDistance)
            {
                swipeDelta.Normalize();
                Vector3 worldDirection = mainCamera.transform.TransformDirection(new Vector3(swipeDelta.x, 0, swipeDelta.y));

                worldDirection.y = verticalAngle; // Adjust this value to control the downward angle
                worldDirection.Normalize(); // Re-normalize to ensure consistent force magnitude

                float swipeSpeed = swipeDistance / 100f;

                ApplyForceToRagdoll(worldDirection, swipeSpeed);

                // Trigger the "pop off" event if the swipe force is sufficient
                if (swipeSpeed * forceMultiplier >= minForceForPopOff)
                {
                    // Check for collisions with NPCs or enemies
                    CheckForCollisions(worldDirection, swipeSpeed * forceMultiplier);
                }
            }
        }
    }

    void ApplyForceToRagdoll(Vector3 direction, float speed)
    {
        if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z) ||
            float.IsInfinity(direction.x) || float.IsInfinity(direction.y) || float.IsInfinity(direction.z) ||
            float.IsNaN(speed) || float.IsInfinity(speed))
        {
            return;
        }

        ForceMode selectedForceMode = ConvertForceMode(forceMode);

        if (!applyToAllParts)
        {
            // Apply force to this object's Rigidbody
            if (playerRb != null)
            {
                playerRb.AddForce(direction * speed * forceMultiplier, selectedForceMode);
                if (addRandomForce)
                {
                    Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    playerRb.AddForce(randomDirection * speed * randomForceMultiplier, selectedForceMode);
                }
            }
        }
        else
        {
            Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.AddForce(direction * speed * forceMultiplier, selectedForceMode);
                // Also apply a force in a random direction
                if (addRandomForce)
                {
                    Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    rb.AddForce(randomDirection * speed * randomForceMultiplier, selectedForceMode);
                }
            }
        }
    }

    void CheckForCollisions(Vector3 direction, float force)
    {
        // Check for collisions with NPCs or enemies
        Collider[] hitColliders = Physics.OverlapSphere(playerRb.position, 1f); // Adjust the radius as needed
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("NPC") || collider.CompareTag("Enemy"))
            {
                // Calculate the collision force
                float collisionForce = force;

                // Trigger the "pop off" event if the collision force is sufficient
                if (collisionForce >= minForceForPopOff)
                {
                    Villager npc = collider.GetComponent<Villager>();
                    Enemy enemy = collider.GetComponent<Enemy>();

                    if (npc != null)
                    {
                        npc.Hit(); // Call Hit() on the Villager
                    }
                    if (enemy != null)
                    {
                        enemy.TakeDamage(10, direction, force); // Call TakeDamage() on the Enemy
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with an NPC or enemy
        if (collision.collider.CompareTag("NPC") || collision.collider.CompareTag("Enemy"))
        {
            // Calculate the collision force
            float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

            // Trigger the "pop off" event if the collision force is sufficient
            if (collisionForce >= minForceForPopOff)
            {
                Villager npc = collision.collider.GetComponent<Villager>();
                Enemy enemy = collision.collider.GetComponent<Enemy>();

                if (npc != null)
                {
                    npc.Hit(); // Call Hit() on the Villager
                }
                if (enemy != null)
                {
                    enemy.TakeDamage(10, collision.contacts[0].normal, collisionForce); // Call TakeDamage() on the Enemy
                }
            }
        }
    }

    ForceMode ConvertForceMode(ForceModeSelector mode)
    {
        switch (mode)
        {
            case ForceModeSelector.Force:
                return ForceMode.Force;
            case ForceModeSelector.Impulse:
                return ForceMode.Impulse;
            case ForceModeSelector.VelocityChange:
                return ForceMode.VelocityChange;
            case ForceModeSelector.Acceleration:
                return ForceMode.Acceleration;
            default:
                return ForceMode.Impulse;
        }
    }
}