using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // prefab del enemigo
    public GameObject enemyPrefab;

    // puntos donde pueden aparecer
    public Transform[] spawnPoints;

    // cantidad de enemigos a generar
    public int enemiesToSpawn = 3;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (spawnPoints.Length == 0) return;

        // evita repetir el mismo punto
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