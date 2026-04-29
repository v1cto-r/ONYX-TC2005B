using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUp;
    float maxHeight=2.5f;
    float minHeight= -3.5f;
    float timeToSpawnMin=1f;
    float timeToSpawnMax=1f;

    //float timeToSpawnMin=10f;
    //float timeToSpawnMax=20f;

    void Start()
    {
        StartCoroutine(SpawnerTime());
    }

    IEnumerator SpawnerTime()   
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeToSpawnMin, timeToSpawnMax));

        Instantiate(powerUp, new Vector3(transform.position.x,transform.position.y+UnityEngine.Random.Range(minHeight, maxHeight), 0), quaternion.identity);

        StartCoroutine(SpawnerTime());
    }
}