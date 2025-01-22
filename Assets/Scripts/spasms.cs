// he tries to run
// but he can't
using UnityEngine;

public class SpasmGenerator : MonoBehaviour
{
    public float strength = 10f;
    public float spasmFrequency = 1f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spasmFrequency)
        {
            spasm(strength);
            timer = 0f;
        }
    }

    void spasm(float strength)
    {
        Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f), 
                Random.Range(-1f, 1f)
            );
            rb.AddForce(randomDirection * strength, ForceMode.Impulse);
        }
    }
}
