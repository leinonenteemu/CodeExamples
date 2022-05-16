using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName ="Enemy Table", menuName = "Scriptable Objects/EnemySpawnTable")]
public class EnemyTable : ScriptableObject
{
    [SerializeField] private List<Enemy> enemies;
    public List<Enemy> Enemies { get => enemies; }


    public Enemy GetRandomEnemy(float currentDifficulty)
    {
        float diceRoll = Random.Range(0, 100);
        Enemy tempEnemy;

        foreach (var enemy in enemies)
        {
            if (enemy.currentSpawnChance >= diceRoll)
            {
                return enemy;
            }
        }

        tempEnemy = enemies[0];
        return tempEnemy;
    }

    public void SortEnemyList()
    {
        enemies.Sort();
    }

}




[System.Serializable]
public class Enemy: System.IComparable<Enemy>
{
    [SerializeField] private GameObject enemyPrefab;
    public GameObject EnemyPrefab { get => enemyPrefab; } 
    [field: SerializeField] public float currentSpawnChance { get; private set; }
    [SerializeField][Range(0, 100)] private float maxSpawnChance;
    [SerializeField][Range(0, 100)] private float maxSpawnChanceReachedAt;

    public void CalculateSpawnChance(float difficulty)
    {
        float percentage = difficulty / maxSpawnChanceReachedAt;
        currentSpawnChance = maxSpawnChance * percentage;
        if (currentSpawnChance > maxSpawnChance) currentSpawnChance = maxSpawnChance;
        
    }

    public int CompareTo(Enemy other)
    {
        if (this.currentSpawnChance > other.currentSpawnChance) return 1;
        else if (this.currentSpawnChance == other.currentSpawnChance) return 0;
        else return -1;
    }
}
