using UnityEngine;

public class WasdHim : MonoBehaviour
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
    public bool applyToAllParts = true;
    public bool addRandomForce = true;
    public float randomForceMultiplier = 3f;
    public ForceModeSelector forceMode = ForceModeSelector.Impulse;

    public Rigidbody playerRb; // Reference to the Rigidbody component
    public GameObject player;

    // Singleton instance
    public static WasdHim Instance { get; private set; }

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
        HandleWASDInput();
    }

    void HandleWASDInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += mainCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= mainCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= mainCamera.transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += mainCamera.transform.right;
        }

        direction.y = 0; // Ensure the direction is horizontal
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            ApplyForceToRagdoll(direction, 1f); // Speed can be adjusted as needed
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