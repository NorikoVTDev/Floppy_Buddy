using UnityEngine;

public class SmoothRotateTorso : MonoBehaviour
{
    public float maxRotationSpeed = 100f; // Maximum rotation speed
    public float rampUpTime = 2f;         // Time to reach max speed
    public float rampDownTime = 2f;       // Time to slow down to 0

    private float currentSpeed = 0f;      // Current rotation speed
    private float targetSpeed = 0f;      // Target rotation speed

    void Update()
    {
        // Determine the target speed based on input
        if (Input.GetKey(KeyCode.Q))
        {
            targetSpeed = maxRotationSpeed; // Rotate left
        }
        else if (Input.GetKey(KeyCode.E))
        {
            targetSpeed = -maxRotationSpeed; // Rotate right
        }
        else
        {
            targetSpeed = 0f; // No input, slow down
        }

        // Adjust the ramp rates dynamically
        float rampRate = (targetSpeed == 0f) ?
            maxRotationSpeed / Mathf.Max(rampDownTime, 0.01f) : // Ramp down
            maxRotationSpeed / Mathf.Max(rampUpTime, 0.01f);   // Ramp up

        // Smoothly adjust current speed toward target speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rampRate * Time.deltaTime);

        // Apply the rotation
        RotateTorso(Vector3.up, currentSpeed);
    }

    private void RotateTorso(Vector3 axis, float speed)
    {
        // Rotate the torso around the given axis with the calculated speed
        transform.Rotate(axis * speed * Time.deltaTime, Space.Self);
    }
}
