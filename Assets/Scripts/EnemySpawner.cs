using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnDistance = 10f; // Distance from the spawner the player can be for an enemy to spawn
    public int targetKills = 5;
    public int currentKills = 0;
    public int currentLevel = 1;
    public int maxEnemiesInLevel = 3;

    private int currentEnemiesInLevel = 0;
    private float spawnCooldown = 5f;
    private float lastSpawnTime = 0f;

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + new Vector3(UnityEngine.Random.Range(-4f, 4f), 0, UnityEngine.Random.Range(-4f, 4f));

        Instantiate(enemyPrefab, spawnPosition, transform.rotation);
        currentEnemiesInLevel++;
        lastSpawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        // Calculate the distance between the player and spawner
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
