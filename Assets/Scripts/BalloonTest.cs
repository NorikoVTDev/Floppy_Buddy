using UnityEngine;

public class BalloonEffect : MonoBehaviour
{
    public float balloonSpeed = 1.0f; // Speed of the ballooning effect
    public float maxExpansion = 1.5f; // Maximum expansion scale
    public SkinnedMeshRenderer skinnedMeshRenderer; // Assign the SkinnedMeshRenderer here

    private Mesh mesh;
    private Vector3[] originalVertices;
    private bool isBallooning = false; // Control if ballooning is active
    private float effectProgress = 0.0f; // Track the progress of the effect

    void Start()
    {
        // Create a copy of the shared mesh to modify it dynamically
        mesh = Instantiate(skinnedMeshRenderer.sharedMesh);
        skinnedMeshRenderer.sharedMesh = mesh;

        // Store the original vertex positions
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        // Trigger the ballooning effect when the B key is pressed
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBallooning = true;
            effectProgress = 0.0f; // Reset the effect progress
        }

        if (isBallooning)
        {
            BalloonCharacter();
        }
    }

    void BalloonCharacter()
    {
        Vector3[] vertices = mesh.vertices;

        // Gradually increase the effect over time
        effectProgress += Time.deltaTime * balloonSpeed;

        // Calculate the current expansion based on effect progress
        float currentExpansion = Mathf.Sin(effectProgress) * maxExpansion;

        // Modify the vertices to create the ballooning effect
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 direction = originalVertices[i].normalized;
            vertices[i] = originalVertices[i] + direction * currentExpansion;
        }

        // Update the mesh with the modified vertices
        mesh.vertices = vertices;
        mesh.RecalculateNormals();

        // Update the SkinnedMeshRenderer bounds manually
        UpdateBounds();

        // Stop the effect after a full sine wave (π radians)
        if (effectProgress >= Mathf.PI)
        {
            isBallooning = false;

            // Reset the vertices to their original positions
            mesh.vertices = originalVertices;
            mesh.RecalculateNormals();
            UpdateBounds();
        }
    }

    void UpdateBounds()
    {
        // Calculate new bounds
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Vector3[] vertices = mesh.vertices;

        foreach (Vector3 vertex in vertices)
        {
            bounds.Encapsulate(vertex);
        }

        // Apply the new bounds to the SkinnedMeshRenderer
        skinnedMeshRenderer.localBounds = bounds;
    }
}
