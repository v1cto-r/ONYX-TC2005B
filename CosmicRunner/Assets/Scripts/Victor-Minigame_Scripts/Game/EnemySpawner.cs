using System.Collections;
using UnityEngine;

namespace AB {

    [System.Serializable]
    public struct EnemyDebris
    {
        public GameObject debrisPrefab;
        [Range(1, 3)]
        public int dificulty;
        [Range(1,2)]
        public int hardness;
        public float speed;
    }

public class EnemySpawner : Spawner
    {
        [Header("Enemy Spawning")]
        public EnemyDebris[] enemiesToSpawn;
        public float homingSpread = 0.1f; // En radianes
        private float debrisWeightSum;

        
        

        void Start()
        {
            Init(spawnInterval, angleLimits, GameController.Instance.gameDuration);

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                debrisWeightSum += 1f / enemiesToSpawn[i].dificulty;
            }

            StartCoroutine(ShipBiasedSpawnEnemies());
        }

        int InverseWeightedRandomEnemy()
        {
            float randomValue = Random.Range(0f, debrisWeightSum);
            float cumulativeWeight = 0f;

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                float weight = 1f / enemiesToSpawn[i].dificulty;
                cumulativeWeight += weight;

                if (randomValue < cumulativeWeight)
                    return i;
            }

            return 0;
        }

        IEnumerator ShipBiasedSpawnEnemies ()
        {
            yield return new WaitForSeconds(this.CalculateSpawnInterval());

            EnemyDebris enemyToSpawn = enemiesToSpawn[InverseWeightedRandomEnemy()];

            float spawnAngle;

            if (enemyToSpawn.hardness == 1)
            {
                float shipAngle = GameController.Instance.shipController.GetShipAngle();
                spawnAngle = shipAngle + Random.Range(-homingSpread, homingSpread);
            } else
            {
                spawnAngle = Random.Range(this.GetAngleLimits().lowerBound, this.GetAngleLimits().upperBound);
            }


            GameObject enemyInstance = Instantiate(enemyToSpawn.debrisPrefab, this.GetSpawnPosition(), Quaternion.Euler(0f, 0f, spawnAngle * Mathf.Rad2Deg), transform);
            EnemyController enemyObject = enemyInstance.GetComponent<EnemyController>();
            enemyObject.Init(enemyToSpawn.hardness, enemyToSpawn.speed, enemyObject.transform.GetChild(0));

            StartCoroutine(ShipBiasedSpawnEnemies());
        }
    }
}
