using UnityEngine;

public class DragCharacter : MonoBehaviour
{
    private Camera mainCamera;
    private Transform selectedObject;
    private Plane dragPlane;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Left mouse button clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if the clicked object has a specific tag or component
                if (hit.collider.CompareTag("Draggable"))
                {
                    selectedObject = hit.collider.transform;

                    // Create a plane at the object's position
                    dragPlane = new Plane(Vector3.up, selectedObject.position);

                    // Calculate the offset between the mouse hit point and the object's position
                    dragPlane.Raycast(ray, out float enter);
                    offset = selectedObject.position - ray.GetPoint(enter);
                }
            }
        }

        // Dragging with the left mouse button
        if (Input.GetMouseButton(0) && selectedObject != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                // Update the object's position based on the mouse position
                selectedObject.position = ray.GetPoint(enter) + offset;
            }
        }

        // Release the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
        }
    }
}
