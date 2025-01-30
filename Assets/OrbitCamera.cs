using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 3.0f;
    public float zoomSpeed = 2.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 15.0f;
    public float smoothSpeed = 0.125f;

    private Vector3 offset;
    private float currentZoom = 10.0f;

    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position; // Initial offset
            currentZoom = offset.magnitude; // Set the starting zoom level
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        // Handle camera rotation with right-click
        if (Input.GetMouseButton(1)) // Right click
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = Input.GetAxis("Mouse Y");

            // Rotate the camera around the target
            offset = Quaternion.AngleAxis(horizontalInput * rotationSpeed, Vector3.up) * offset;
            offset = Quaternion.AngleAxis(-verticalInput * rotationSpeed, transform.right) * offset;
        }

        // Handle zoom with the mouse wheel
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= zoomInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Update the camera's position based on offset and zoom
        Vector3 desiredPosition = target.position + offset.normalized * currentZoom;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera look at the target
        transform.LookAt(target);
    }
}
