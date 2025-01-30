using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public RagdollEnabler ragdollEnabler;
    public float minCollisionForce = 10f; // Minimum force required to trigger detection
    public GameObject targetRagdoll; // Public field to assign the specific ragdoll

    public void SetTargetRagdoll(GameObject targetRagdoll)
    {
        this.targetRagdoll = targetRagdoll;
    }

    private void OnTriggerEnter(Collider player)
    {
        // Check if the colliding object is the specific ragdoll or part of it
        if (player.gameObject == targetRagdoll)
        {
            // Get the Rigidbody of the colliding object
            Rigidbody ragdollRigidbody = player.GetComponent<Rigidbody>();

            if (ragdollRigidbody != null)
            {
                // Calculate the collision force (magnitude of velocity)
                float collisionForce = ragdollRigidbody.linearVelocity.magnitude;

                // Check if the force is above the threshold
                if (collisionForce >= minCollisionForce)
                {
                    Debug.Log("Specific ragdoll hit the enemy with enough force: " + collisionForce);
                    ragdollEnabler.EnableRagdoll();
                }
            }
        }
    }
}