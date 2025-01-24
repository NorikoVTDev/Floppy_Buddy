using UnityEngine;

public class RagdollRoll : MonoBehaviour
{
    public float rollForce = 1000f; // Strong roll force
    public float liftForce = 5f;   // Lift force to help overcome ground friction
    private Rigidbody rb;          // Rigidbody reference

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on the ragdoll!");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            PerformRoll(Vector3.forward); // Roll left
        }
        else if (Input.GetKey(KeyCode.E))
        {
            PerformRoll(Vector3.back); // Roll right
        }
    }

    private void PerformRoll(Vector3 direction)
    {
        if (rb != null)
        {
            // Apply upward lift to reduce friction with the ground
            rb.AddForce(Vector3.up * liftForce, ForceMode.Impulse);

            // Apply torque to roll the character
            rb.AddTorque(direction * rollForce, ForceMode.Impulse);
        }
    }
}
