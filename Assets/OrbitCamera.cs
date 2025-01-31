using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 3.0f;
    public float zoomSpeed = 2.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;
    public float smoothSpeed = 2f;
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;
    public LayerMask obstacleMask;

    private Vector3 offset;
    private float currentZoom = 10.0f;
    private float currentXRotation = 0f;

    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
            currentZoom = offset.magnitude;
            
            // Calculate initial X rotation
            currentXRotation = transform.eulerAngles.x;
            if (currentXRotation > 180)
                currentXRotation -= 360;
        }
    }

    private void Update()
    {
        if (target == null)
            return;

        HandleRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            // Handle vertical rotation with clamping
            currentXRotation -= mouseY;
            currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);

            // Create the rotation we want to apply to the offset
            Quaternion xRotation = Quaternion.Euler(currentXRotation, 0, 0);
            Quaternion yRotation = Quaternion.Euler(0, mouseX, 0);

            // Apply rotations in the correct order
            offset = yRotation * offset;
            Vector3 right = Vector3.Cross(Vector3.up, offset.normalized);
            offset = Quaternion.AngleAxis(mouseY, right) * offset;

            // Ensure we stay within vertical angle limits
            Vector3 forward = -offset.normalized;
            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(forward, Vector3.up), forward, right);
            if (angle < minVerticalAngle || angle > maxVerticalAngle)
            {
                offset = Quaternion.AngleAxis(-mouseY, right) * offset;
            }
        }
    }

    private void HandleZoom()
    {
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= zoomInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    private void UpdateCameraPosition()
    {
        Vector3 desiredPosition = target.position + offset.normalized * currentZoom;

        // Check for obstacles
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit, obstacleMask))
        {
            desiredPosition = hit.point - offset.normalized * 0.5f;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target);

        // Smoothly move the camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Calculate the desired rotation to look at target
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        
        // Smoothly rotate the camera
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}