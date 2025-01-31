using UnityEngine;

public class WavesController : MonoBehaviour
{
    [Header("Wave Settings")]
    public int initialKillsRequired = 5; // Number of kills required to advance to the next wave
    public int killsIncreasePerWave = 5; // How much the required kills increase per wave
    public int currentWave = 1; // Current wave number

    [Header("Enemy Spawner")]
    public EnemySpawner enemySpawner; // Reference to the EnemySpawner

    private int currentKills = 0; // Tracks the number of enemies killed in the current wave

    private void Start()
    {
        // Initialize the wave system
        InitializeWave();
    }

    private void InitializeWave()
    {
        // Set the initial kills required for the first wave
        enemySpawner.targetKills = initialKillsRequired;
        enemySpawner.currentKills = 0;
        enemySpawner.currentLevel = currentWave;

        Debug.Log($"Wave {currentWave} started! Kills required: {initialKillsRequired}");
    }

    public void EnemyKilled()
    {
        // Increment the kill count
        currentKills++;

        // Check if the required kills for the wave have been reached
        if (currentKills >= initialKillsRequired)
        {
            AdvanceWave();
        }
    }

    private void AdvanceWave()
    {
        // Increment the wave number
        currentWave++;

        // Increase the required kills for the next wave
        initialKillsRequired += killsIncreasePerWave;

        // Reset the kill count for the new wave
        currentKills = 0;

        // Update the EnemySpawner with the new wave settings
        enemySpawner.targetKills = initialKillsRequired;
        enemySpawner.currentKills = 0;
        enemySpawner.currentLevel = currentWave;

        Debug.Log($"Wave {currentWave} started! Kills required: {initialKillsRequired}");
    }
}
