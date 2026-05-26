using UnityEngine;
using System.Collections;

namespace AB
{
    [System.Serializable]
    public struct Boosters
    {
        public GameObject boosterPrefab;
        [Range(1, 5)]
        public int availability;
    }

    public class BoosterSpawner : Spawner
    {
        [Header("Enemy Spawning")]
        public Boosters[] boostersToSpawn;
        public float boosterSpeed = 5f;
        private float boosterWeightSum;
        

        void Start()
        {
            Init(spawnInterval, angleLimits, GameController.Instance.gameDuration);

            // El acumulado de dificultad, para generar enemigos en base a su dificultad
            // Se va a calcular de manera inversa para que de tal manera
            // > dificultad = < probabilidad de spawn
            for (int i = 0; i < boostersToSpawn.Length; i++)
            {
                boosterWeightSum += boostersToSpawn[i].availability;
            }

            StartCoroutine(SpawnBoosters());
        }

        int InverseWeightedRandomEnemy()
        {
            // Genera un número aleatorio con probabilidades ponderadas
            // Genera un número entre 0 y la suma de las probabilidades acumuladas
            float randomValue = Random.Range(0f, boosterWeightSum);
            float cumulativeWeight = 0f;

            for (int i = 0; i < boostersToSpawn.Length; i++)
            {
                // El peso calculado inverso a la dificultad
                float weight = boostersToSpawn[i].availability;
                cumulativeWeight += weight;

                if (randomValue < cumulativeWeight)
                    return i;
            }

            return 0;
        }

        IEnumerator SpawnBoosters ()
        {
            yield return new WaitForSeconds(this.CalculateSpawnInterval());

            Boosters boosterToSpawn = boostersToSpawn[InverseWeightedRandomEnemy()];

            float spawnAngle = Random.Range(this.GetAngleLimits().lowerBound, this.GetAngleLimits().upperBound);

            GameObject boosterInstance = Instantiate(boosterToSpawn.boosterPrefab, this.GetSpawnPosition(), Quaternion.Euler(0f, 0f, spawnAngle * Mathf.Rad2Deg), transform);
            BoosterController boosterObject = boosterInstance.GetComponent<BoosterController>();
            boosterObject.Init(boosterSpeed);

            StartCoroutine(SpawnBoosters());
        }
    }   
}
