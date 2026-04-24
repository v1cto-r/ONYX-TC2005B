using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject bigEnemyPrefab; // Second enemy option

    public float maxHeight;
    public float minHeight;
    public float timeToSpawnMin;
    public float timeToSpawnMax;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnerTime());
    }

    IEnumerator SpawnerTime()
    {
        yield return new WaitForSeconds(Random.Range(timeToSpawnMin, timeToSpawnMax));

        // Pick which enemy to spawn
        GameObject prefabToSpawn = enemyPrefab;
        if (bigEnemyPrefab != null)
        {
            // Both loaded, pick randomly
            prefabToSpawn = Random.value < 0.15f ? bigEnemyPrefab : enemyPrefab;
        }

        Instantiate(prefabToSpawn, new Vector3(transform.position.x, 
            transform.position.y + Random.Range(minHeight, maxHeight), 0), Quaternion.identity);
        
        StartCoroutine(SpawnerTime());
    }
}
