using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Transform[] enemySpawnPoints;

    [SerializeField] List<GameObject> activeEnemies = new List<GameObject>();
    [SerializeField] List<GameObject> enemyPool = new List<GameObject>();
    private int enemiesSpawned;
    [SerializeField] private int maxEnemiesAtOnce;
    private int enemiesKilledThisRound;
    private int totalEnemiesKilled = 0;
    WaveManager waveManager;

    public bool maxDifficulty { get; private set; }

    [SerializeField] float difficultyIncreasePerWave = 5;

    [SerializeField][Range(0, 100)] float currentDifficulty = 90f;

    [SerializeField] EnemyTable enemyTable;
    // Start is called before the first frame update
    void Start()
    {
        waveManager = GetComponent<WaveManager>();
        ResetOnNewWave();
        CalculateEnemySpawnChances();
    }

    // Update is called once per frame
    void Update()
    {
        if (waveManager.waveActive)
        {
            if (enemiesSpawned < waveManager.EnemiesInThisWave)
            {
                if (activeEnemies.Count < maxEnemiesAtOnce)
                {
                    SpawnEnemy();
                }
            }
        }

    }

    public bool AllEnemiesKilled()
    {
        if (enemiesSpawned >= waveManager.EnemiesInThisWave && enemiesKilledThisRound >= enemiesSpawned) return true;
        else return false;
    }

    public void ResetOnNewWave()
    {
        enemiesSpawned = 0;
        enemiesKilledThisRound = 0;
    }


    void SpawnEnemy()
    {
        if (enemiesSpawned >= waveManager.EnemiesInThisWave) return;
        enemiesSpawned++;
        bool enemyTakenFromPool = false;
        Enemy enemyType = enemyTable.GetRandomEnemy(currentDifficulty);
        Debug.Log(enemyType.EnemyPrefab.name);
        GameObject enemy;
        int spawnPoint = Random.Range(0, enemySpawnPoints.Length);
        if (enemyPool.Count > 0)
        {
            if (FindEnemyFromPool(enemyType, out enemy))
            {
                enemy.SetActive(true);
                enemyPool.Remove(enemy);
                enemy.transform.position = enemySpawnPoints[spawnPoint].position;
                activeEnemies.Add(enemy);
                enemyTakenFromPool = true;
            }
        }
        if (!enemyTakenFromPool)
        {
            enemy = Instantiate(enemyType.EnemyPrefab, enemySpawnPoints[spawnPoint].position, Quaternion.identity);
            enemy.name = enemyType.EnemyPrefab.name;
            activeEnemies.Add(enemy);
        }
    }

    private bool FindEnemyFromPool(Enemy enemyType, out GameObject enemy)
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (enemyPool[i].name == enemyType.EnemyPrefab.name)
            {
                enemy = enemyPool[i];
                return true;
            }
        }
        enemy = null;
        return false;
    }

    public void CalculateEnemySpawnChances()
    {
        foreach (Enemy enemy in enemyTable.Enemies)
        {
            enemy.CalculateSpawnChance(currentDifficulty);
        }
        enemyTable.SortEnemyList();
    }

    public void AddToEnemiesKilled()
    {
        enemiesKilledThisRound++;
        totalEnemiesKilled++;
    }

    public void AddEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        if (activeEnemies.Contains(enemy)) activeEnemies.Remove(enemy);
        enemyPool.Add(enemy);
    }

    public int GetTotalEnemiesKilled()
    {
        return totalEnemiesKilled;
    }

    public void IncreaseDifficulty()
    {
        if (!maxDifficulty) 
        {
            currentDifficulty += difficultyIncreasePerWave;
            enemyTable.SortEnemyList();

            if (currentDifficulty > 100)
            {
                currentDifficulty = 100;
                enemyTable.SortEnemyList();
                maxDifficulty = true;
            }
        }
    }
}
