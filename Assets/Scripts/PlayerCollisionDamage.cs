using UnityEngine;

public class PlayerCollisionDamage : MonoBehaviour
{
    public float minForceForDamage = 5f; // Minimum force required to deal damage
    public float damageMultiplier = 0.1f; // Damage = force * damageMultiplier

    public Rigidbody playerRb;
    public float forceStrength = 2.0f;
    public Vector3 forceDirection;

    private void Start()
    {
        // Access the playerRb reference from the swipeHim singleton
        if (SwipeHim.Instance != null)
        {
            playerRb = SwipeHim.Instance.playerRb;
        }
        else
        {
            Debug.LogError("swipeHim singleton instance not found!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime; // Calculate force
            if (collisionForce >= minForceForDamage)
            {
                int damage = Mathf.RoundToInt(collisionForce * damageMultiplier);
                if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(damage, forceDirection, forceStrength);
                    Debug.Log($"Player collided with enemy for {damage} damage!");
                }
            }
        }
    }
}
