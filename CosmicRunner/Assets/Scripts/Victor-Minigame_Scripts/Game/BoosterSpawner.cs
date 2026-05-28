using UnityEngine;
using System.Collections;

namespace AB
{
    [System.Serializable]
    public struct Boosters
    {
        // El prefab del booster a spawnear
        public GameObject boosterPrefab;

        // Que tan probable es que spawnee el booster
        [Range(1, 5)]
        public int availability;
    }

    public class BoosterSpawner : Spawner
    {
        [Header("Enemy Spawning")]
        public Boosters[] boostersToSpawn; // Boosters
        public float boosterSpeed = 5f; // Su velocidad (Aplica a todos)
        private float boosterWeightSum; // Usado para calcular la probabilidad
        

        void Start()
        {
            // Manda a llamar el init de la clase padre Spawner
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

        // Calcula el booster a spawnear usando un random ponderado
        // En base al valor de disponibilidad
        int WeightedRandomBooster()
        {
            // Genera un número aleatorio con probabilidades ponderadas
            // Genera un número entre 0 y la suma de las probabilidades acumuladas
            float randomValue = Random.Range(0f, boosterWeightSum);
            float cumulativeWeight = 0f;

            for (int i = 0; i < boostersToSpawn.Length; i++)
            {
                // El peso calculado
                float weight = boostersToSpawn[i].availability;
                cumulativeWeight += weight;

                if (randomValue < cumulativeWeight)
                    return i;
            }

            return 0;
        }

        // Coroutine para spawnear los boosters por tiempo interpolado
        // Definido en spawner
        IEnumerator SpawnBoosters ()
        {
            yield return new WaitForSeconds(this.CalculateSpawnInterval());

            Boosters boosterToSpawn = boostersToSpawn[WeightedRandomBooster()];

            float spawnAngle = Random.Range(this.GetAngleLimits().lowerBound, this.GetAngleLimits().upperBound);

            GameObject boosterInstance = Instantiate(boosterToSpawn.boosterPrefab, this.GetSpawnPosition(), Quaternion.Euler(0f, 0f, spawnAngle * Mathf.Rad2Deg), transform);
            BoosterController boosterObject = boosterInstance.GetComponent<BoosterController>();
            boosterObject.Init(boosterSpeed);

            StartCoroutine(SpawnBoosters());
        }
    }   
}
