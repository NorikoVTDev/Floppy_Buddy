using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public void BuildNavMesh()
    {
        // Define the bounds of the NavMesh
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(100, 100, 100)); // Adjust as needed

        // Collect sources for the NavMesh
        NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0); // Use default settings
        NavMeshBuildMarkup[] markups = new NavMeshBuildMarkup[0]; // No additional markups
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(
            bounds,
            LayerMask.GetMask("Ground"),
            NavMeshCollectGeometry.PhysicsColliders,
            0,
            new List<NavMeshBuildMarkup>(),
            sources
        );

        // Build the NavMesh
        NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(
            buildSettings, // NavMesh build settings
            sources, // Collected sources
            bounds, // Bounds for the NavMesh
            Vector3.zero, // Position offset
            Quaternion.identity // Rotation offset
        );

        // Add the NavMesh to the scene
        NavMesh.AddNavMeshData(navMeshData);
    }
}
