using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    // puntos donde pueden aparecer enemigos
    public Transform[] spawnPoints;

    // cuantos enemigos quieres
    public int enemiesToSpawn = 3;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (spawnPoints.Length == 0) return;

        // evitar repetir el mismo punto
        bool[] used = new bool[spawnPoints.Length];

        int spawned = 0;

        while (spawned < enemiesToSpawn)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);

            if (!used[randomIndex])
            {
                Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
                used[randomIndex] = true;
                spawned++;
            }
        }
    }
}