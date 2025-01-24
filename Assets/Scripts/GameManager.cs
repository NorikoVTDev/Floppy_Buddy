using UnityEngine;

public class GameManager : MonoBehaviour
{
    public NavMeshGenerator navMeshGenrator;

    private void Start()
    {
        // Generate the NavMesh at runtime
        navMeshGenrator.BuildNavMesh();
    }
}
