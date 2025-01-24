using UnityEngine;

public class RagdollHelper : MonoBehaviour
{
    [System.Serializable]
    public class BodyPart
    {
        public Transform transform;
        public Vector3 colliderCenter;
        public Vector3 colliderSize;
        public float colliderRadius;
        public bool isSpherical; // True for spherical colliders (e.g., head), false for box colliders
    }

    public BodyPart[] bodyParts;

    private Rigidbody[] ragdollRigidbodies;

    public void EnableRagdoll(bool enabled)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            if (rb != null) rb.isKinematic = !enabled;
        }

        // Disable the main Collider and Rigidbody when in ragdoll mode
        Collider mainCollider = GetComponent<Collider>();
        Rigidbody mainRigidbody = GetComponent<Rigidbody>();

        if (mainCollider != null) mainCollider.enabled = !enabled;
        if (mainRigidbody != null) mainRigidbody.isKinematic = enabled;
    }

    public Rigidbody[] GetRagdollRigidbodies()
    {
        return ragdollRigidbodies;
    }

    public void SetupRagdoll()
    {
        ragdollRigidbodies = new Rigidbody[bodyParts.Length];

        for (int i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i].transform == null)
            {
                Debug.LogError($"Body part transform is null: {bodyParts[i].transform}");
                continue;
            }

            // Add Rigidbody
            Rigidbody rb = bodyParts[i].transform.gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f; // Adjust mass as needed
            rb.linearDamping = 0.1f;
            rb.angularDamping = 0.1f;

            // Add Collider
            if (bodyParts[i].isSpherical)
            {
                SphereCollider sphereCollider = bodyParts[i].transform.gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = bodyParts[i].colliderRadius;
                sphereCollider.center = bodyParts[i].colliderCenter;
            }
            else
            {
                BoxCollider boxCollider = bodyParts[i].transform.gameObject.AddComponent<BoxCollider>();
                boxCollider.size = bodyParts[i].colliderSize;
                boxCollider.center = bodyParts[i].colliderCenter;
            }

            // Add CharacterJoint (if not the root body part)
            if (bodyParts[i].transform != transform)
            {
                CharacterJoint joint = bodyParts[i].transform.gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = bodyParts[i].transform.parent.GetComponent<Rigidbody>();
            }

            ragdollRigidbodies[i] = rb;
        }
    }
}