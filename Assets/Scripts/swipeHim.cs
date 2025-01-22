using UnityEngine;

public class SwipeHim : MonoBehaviour
{
    public enum ForceModeSelector
    {
        Force,
        Impulse,
        VelocityChange,
        Acceleration
    }

    public Camera mainCamera;
    public float forceMultiplier = 2f;
    public float minSwipeDistance = 50f;
    public bool applyToAllParts = true;
    public bool addRandomForce = true;
    public float randomForceMultiplier = 3f;
    public ForceModeSelector forceMode = ForceModeSelector.Impulse;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private bool isSwiping = false;

    void Update() {
        DetectSwipe();
    }

    void DetectSwipe() {
        if (Input.GetMouseButtonDown(0)) {
            swipeStartPosition = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping) {
            swipeEndPosition = Input.mousePosition;
            isSwiping = false;
            Vector2 swipeDelta = swipeEndPosition - swipeStartPosition;
            float swipeDistance = swipeDelta.magnitude;

            if (swipeDistance >= minSwipeDistance) {
                swipeDelta.Normalize();
                Vector3 worldDirection = mainCamera.transform.TransformDirection(new Vector3(swipeDelta.x, 0, swipeDelta.y));
                worldDirection.y = 0;
                worldDirection.Normalize();

                float swipeSpeed = swipeDistance / 100f;

                ApplyForceToRagdoll(worldDirection, swipeSpeed);
            }
        }
    }

    void ApplyForceToRagdoll(Vector3 direction, float speed) {
        if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z) ||
            float.IsInfinity(direction.x) || float.IsInfinity(direction.y) || float.IsInfinity(direction.z) ||
            float.IsNaN(speed) || float.IsInfinity(speed))
        {
            return;
        }

        ForceMode selectedForceMode = ConvertForceMode(forceMode);

        if (!applyToAllParts){
            // Get the Rigidbody component of this object
            Rigidbody thisRigidbody = gameObject.GetComponent<Rigidbody>();

            thisRigidbody.AddForce(direction * speed * forceMultiplier, selectedForceMode);
            if (addRandomForce) {
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                thisRigidbody.AddForce(randomDirection * speed * randomForceMultiplier, selectedForceMode);
            }
        }
        else {
            Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in ragdollRigidbodies) {
                rb.AddForce(direction * speed * forceMultiplier, selectedForceMode);
                // Also apply a force in a random direction
                if (addRandomForce)
                {
                    Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    rb.AddForce(randomDirection * speed * randomForceMultiplier, selectedForceMode);
                }
            }
        }
    }

    ForceMode ConvertForceMode(ForceModeSelector mode)
    {
        switch (mode)
        {
            case ForceModeSelector.Force:
                return ForceMode.Force;
            case ForceModeSelector.Impulse:
                return ForceMode.Impulse;
            case ForceModeSelector.VelocityChange:
                return ForceMode.VelocityChange;
            case ForceModeSelector.Acceleration:
                return ForceMode.Acceleration;
            default:
                return ForceMode.Impulse;
        }
    }
}