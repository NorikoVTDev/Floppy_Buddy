using UnityEngine;

public class JumpScript : MonoBehaviour
{
    public float jumpForce = 5f; // Adjust the jump force
    private bool isGrounded = true; // Check if the player is grounded
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player is grounded when touching a surface
        if (collision.contacts[0].normal.y > 0.1f)
        {
            isGrounded = true;
        }
    }
}
