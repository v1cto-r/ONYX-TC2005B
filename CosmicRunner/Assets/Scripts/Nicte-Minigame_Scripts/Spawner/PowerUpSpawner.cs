using UnityEngine;
using System.Collections;

namespace Nicte.Minigame{
public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUp;
    float maxHeight=2.5f;
    float minHeight= -3.5f;
    float timeToSpawnMin=5f;
    float timeToSpawnMax=10f;

    void Start()
    {
        StartCoroutine(SpawnerTime());
    }

    IEnumerator SpawnerTime()   
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeToSpawnMin, timeToSpawnMax));

        Instantiate(powerUp, new Vector3(transform.position.x,transform.position.y+UnityEngine.Random.Range(minHeight, maxHeight), 0), Quaternion.identity);

        StartCoroutine(SpawnerTime());
    }
}
}