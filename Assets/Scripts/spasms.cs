// he tries to run
// but he can't

using UnityEngine;

public class TryToRun : MonoBehaviour
{
    public float speed = 10f;
    public float randomForceMultiplier = 3f;
    // time in seconds between spasms
    public float spasmFrequency = 0.5f;

    void Update()
    {
        if (Time.time % spasmFrequency == 0)
        {
            spasm(speed);
        }
    }

    void spasm(float speed)
    {

        Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            rb.AddForce(randomDirection * speed * randomForceMultiplier, ForceMode.Impulse);
        }
    }
}