using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemiesPerWave = 1;
    [SerializeField] private int maxEnemiesPerWave = 3;
    [SerializeField] private float timeToSpawnMin = 1f;
    [SerializeField] private float timeToSpawnMax = 3f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(timeToSpawnMin, timeToSpawnMax));
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        if (enemyPrefab == null || groundTilemap == null || wallsTilemap == null)
        {
            Debug.LogWarning("EnemySpawner is missing references.", this);
            return;
        }

        int clampedMin = Mathf.Max(1, minEnemiesPerWave);
        int clampedMax = Mathf.Max(clampedMin, maxEnemiesPerWave);
        int enemiesToSpawn = Random.Range(clampedMin, clampedMax + 1);

        List<Vector3Int> validTiles = GetValidSpawnTiles();
        int spawnedCount = 0;

        while (spawnedCount < enemiesToSpawn && validTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, validTiles.Count);
            Vector3Int spawnCell = validTiles[randomIndex];
            validTiles.RemoveAt(randomIndex);

            Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(spawnCell);

            if (IsCellOccupied(spawnWorldPos))
            {
                continue;
            }

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnWorldPos, Quaternion.identity);
            EnemyBehaviour enemyBehaviour = spawnedEnemy.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour != null)
            {
                enemyBehaviour.Initialize(groundTilemap, wallsTilemap);
            }

            spawnedCount++;
        }

        if (spawnedCount < enemiesToSpawn)
        {
            Debug.LogWarning("Spawned " + spawnedCount + " out of " + enemiesToSpawn + " enemies because not enough free tiles were available.");
        }
    }

    private List<Vector3Int> GetValidSpawnTiles()
    {
        List<Vector3Int> validTiles = new List<Vector3Int>();

        foreach (Vector3Int cell in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(cell) && !wallsTilemap.HasTile(cell))
            {
                validTiles.Add(cell);
            }
        }

        return validTiles;
    }

    private bool IsCellOccupied(Vector3 worldPosition)
    {
        Collider2D[] collidersAtPoint = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D colliderAtPoint in collidersAtPoint)
        {
            if (colliderAtPoint == null)
            {
                continue;
            }

            if (colliderAtPoint.CompareTag("Player") || colliderAtPoint.CompareTag("Enemy") || colliderAtPoint.CompareTag("Box"))
            {
                return true;
            }
        }

        return false;
    }
}
