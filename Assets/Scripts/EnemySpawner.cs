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
    public int targetKills = 5; // Set by WavesController
    public int currentKills = 0; // Tracks kills in the current wave
    public int currentLevel = 1; // Current wave number
    public int maxEnemiesInLevel = 3; // Max enemies allowed in the current wave
    public SoundController soundController;

    // Internal state
    private int currentEnemiesInLevel = 0;
    private float spawnCooldown = 5f;
    private float lastSpawnTime = 0f;

    // Reference to the WavesController
    public WavesController wavesController;

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

        // Ensure WavesController is assigned
        if (wavesController == null)
        {
            Debug.LogError("WavesController reference is not assigned!");
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + new Vector3(UnityEngine.Random.Range(-4f, 4f), 0, UnityEngine.Random.Range(-4f, 4f));
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, transform.rotation);
        enemy.tag = "Enemy";

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetPlayer(player, playerRagdollRoot);
            enemyScript.SetSpawner(this);
            enemyScript.setSoundController(soundController);
            Debug.Log($"Player reference assigned: {player != null}");
        }
        else
        {
            Debug.LogWarning("Enemy prefab does not have an Enemy script attached.");
        }

        currentEnemiesInLevel++;
        lastSpawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= spawnDistance && currentKills < targetKills && currentEnemiesInLevel < maxEnemiesInLevel && Time.time >= lastSpawnTime + spawnCooldown)
        {
            SpawnEnemy();
        }
    }

    public void EnemyKilled()
    {
        currentKills++;
        currentEnemiesInLevel--;

        if (wavesController != null)
        {
            wavesController.EnemyKilled();
        }
    }
}
