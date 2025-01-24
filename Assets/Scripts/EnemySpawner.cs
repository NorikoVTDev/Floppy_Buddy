using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private Transform player;
    public RuntimeAnimatorController enemyAnimatorController;
    public float spawnDistance = 10f; // Distance from the spawner the player can be for an enemy to spawn
    public int targetKills = 5;
    public int currentKills = 0;
    public int currentLevel = 1;
    public int maxEnemiesInLevel = 3;
    public Rigidbody playerRb;

    private int currentEnemiesInLevel = 0;
    private float spawnCooldown = 5f;
    private float lastSpawnTime = 0f;

    private void Start()
    {
        // Access the playerRb reference from the swipeHim singleton
        if (SwipeHim.Instance != null)
        {
            playerRb = SwipeHim.Instance.playerRb;
            player = playerRb.transform;
        }
        else
        {
            Debug.LogError("swipeHim singleton instance not found!");
        }
    }

    void SpawnEnemy()
    {
        // Randomize the spawn position within a set area
        Vector3 spawnPosition = transform.position + new Vector3(UnityEngine.Random.Range(-4f, 4f), 0, UnityEngine.Random.Range(-4f, 4f));
        
        // Spawn the enemy at chosen position
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);

        // Add required components to the enemy at runtime
        AddComponentsToEnemy(enemy);

        // Get the Enemy component and assign necessary refrences
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.spawner = this;
            enemyScript.player = player;
        }
        else
        {
            Debug.LogWarning("Enemy prefab does not have an Enemy script attached.");
        }

        // Update the enemy count and cooldown
        currentEnemiesInLevel++;
        lastSpawnTime = Time.time;
    }

    void AddComponentsToEnemy(GameObject enemy)
    {
        // Add a NavMeshAgent component
        NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
        agent.speed = 3f;
        agent.stoppingDistance = 1f;

        // Add an Animator component
        Animator animator = enemy.AddComponent<Animator>();
        
        // Assign the Animator Controller at runtime
        if (animator != null && enemyAnimatorController != null)
        {
            animator.runtimeAnimatorController = enemyAnimatorController;
        }

        // Add a Rigidbody component
        Rigidbody rb = enemy.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Add a BoxCollider component (Will switch to mesh collider with new enemy model/mesh
        BoxCollider boxCollider = enemy.AddComponent<BoxCollider>();

        // Add the Enemy Script if it's not already attached
        if (enemy.GetComponent<Enemy>() == null)
        {
            enemy.AddComponent<Enemy>();
        }
    }

    private void FixedUpdate()
    {
        // Calculate the distance between the player and spawner
        float distanceToPlayer = Vector3.Distance(transform.position, playerRb.position);

        // Debug: Log the current state of variables
        Debug.Log($"Distance to Player:{distanceToPlayer}, Current Enemies: {currentEnemiesInLevel}, Max Enemies: {maxEnemiesInLevel}, Cooldown: {Time.time - lastSpawnTime}");

        if (distanceToPlayer <= spawnDistance && currentKills < targetKills && currentEnemiesInLevel < maxEnemiesInLevel && Time.time >= lastSpawnTime + spawnCooldown)
        {
            SpawnEnemy();
        }
    }

    public void EnemyKilled()
    {
        currentKills++;
        currentEnemiesInLevel--;

        if (currentKills >= targetKills)
        {
            AdvanceLevel();
        }
    }

    void AdvanceLevel()
    {
        currentLevel++;
        targetKills += 5;
        maxEnemiesInLevel += 3;

        Debug.Log($"Level Advanced! Current Level: {currentLevel}, Max Enemies: {maxEnemiesInLevel}, Target Kills: {targetKills}");

        // Reset current kills and enemies in level for new level
        currentKills = 0;
    }
}
