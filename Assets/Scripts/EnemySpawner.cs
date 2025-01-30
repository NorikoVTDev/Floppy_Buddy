using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const float Y = 1f;

    // Assign the enemy prefab in the Inspector
    public GameObject enemyPrefab;
    public GameObject playerRagdollRoot;

    // References
    private Transform player;
    public RuntimeAnimatorController enemyAnimatorController;

    // Spawn settings
    public float spawnDistance = 10f; // Distance from the spawner the player can be for an enemy to spawn
    public int targetKills = 5;
    public int currentKills = 0;
    public int currentLevel = 1;
    public int maxEnemiesInLevel = 3;

    // Internal state
    private int currentEnemiesInLevel = 0;
    private float spawnCooldown = 5f;
    private float lastSpawnTime = 0f;

    private void Start()
    {
        // Access the player reference from the swipeHim singleton
        if (SwipeHim.Instance != null)
        {
            player = SwipeHim.Instance.playerRb.transform;
            Debug.Log($"Player reference found: {player != null}");
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

        // Spawn the enemy at the chosen position
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);

        // Assign the "Enemy" tag
        enemy.tag = "Enemy";

        // Get the Enemy component and assign necessary references
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            // Set the player and spawner references
            enemyScript.SetPlayer(player, playerRagdollRoot);
            enemyScript.SetSpawner(this);

            Debug.Log($"Player reference assigned: {player != null}");
        }
        else
        {
            Debug.LogWarning("Enemy prefab does not have an Enemy script attached.");
        }

        // Update the enemy count and cooldown
        currentEnemiesInLevel++;
        lastSpawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        // Calculate the distance between the player and spawner
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Spawn an enemy if conditions are met
        if (distanceToPlayer <= spawnDistance && currentKills < targetKills && currentEnemiesInLevel < maxEnemiesInLevel && Time.time >= lastSpawnTime + spawnCooldown)
        {
            SpawnEnemy();
        }
    }

    public void EnemyKilled()
    {
        currentKills++;
        currentEnemiesInLevel--;

        // Advance to the next level if the target kills are reached
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

        // Reset current kills and enemies in level for the new level
        currentKills = 0;
    }
}
