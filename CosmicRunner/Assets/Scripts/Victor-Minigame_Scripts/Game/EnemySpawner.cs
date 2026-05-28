using System.Collections;
using UnityEngine;

namespace AB {

    [System.Serializable]
    public struct EnemyDebris
    {
        // Prefab del enemigo
        public GameObject debrisPrefab;
        // Dificultad del enemigo, entre 1 y 3, va a afectar la probabilidad de spawn
        [Range(1, 3)]
        public int dificulty;
        // Cuantas balas para destruirlo
        [Range(1,2)]
        public int hardness;
        // Velocidad del enemigo
        public float speed;
    }

public class EnemySpawner : Spawner
    {
        [Header("Enemy Spawning")]
        public EnemyDebris[] enemiesToSpawn;
        // Para cuando el objeto tenga homing
        // Es decir que spawnee con un ángulo directo a la nave
        // Que tanto se puede desviar de ese ángulo
        // Para que parezca natural y no tan directo
        public float homingSpread = 0.1f; // En radianes
        private float debrisWeightSum;

        
        

        void Start()
        {
            // Manda a llamar el init de la clase padre Spawner
            Init(spawnInterval, angleLimits, GameController.Instance.gameDuration);

            // El acumulado de dificultad, para generar enemigos en base a su dificultad
            // Se va a calcular de manera inversa para que de tal manera
            // > dificultad = < probabilidad de spawn
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                debrisWeightSum += 1f / enemiesToSpawn[i].dificulty;
            }

            StartCoroutine(ShipBiasedSpawnEnemies());
        }

        int InverseWeightedRandomEnemy()
        {
            // Genera un número aleatorio con probabilidades ponderadas
            // Genera un número entre 0 y la suma de las probabilidades acumuladas
            float randomValue = Random.Range(0f, debrisWeightSum);
            float cumulativeWeight = 0f;

            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                // El peso calculado inverso a la dificultad
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

            // Si su hardness es 1, va a tener homing
            if (enemyToSpawn.hardness == 1)
            {
                float shipAngle = GameController.Instance.shipController.GetShipAngle();
                spawnAngle = shipAngle + Random.Range(-homingSpread, homingSpread);
            } else
            {
                // Spawnea en un ángulo aleatorio dentro de los límites definidos
                spawnAngle = Random.Range(this.GetAngleLimits().lowerBound, this.GetAngleLimits().upperBound);
            }


            GameObject enemyInstance = Instantiate(enemyToSpawn.debrisPrefab, this.GetSpawnPosition(), Quaternion.Euler(0f, 0f, spawnAngle * Mathf.Rad2Deg), transform);
            EnemyController enemyObject = enemyInstance.GetComponent<EnemyController>();
            enemyObject.Init(enemyToSpawn.hardness, enemyToSpawn.speed, enemyObject.transform.GetChild(0));

            StartCoroutine(ShipBiasedSpawnEnemies());
        }
    }
}
