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

            for (int i = 0; i < boostersToSpawn.Length; i++)
            {
                boosterWeightSum += boostersToSpawn[i].availability;
            }

            StartCoroutine(SpawnBoosters());
        }

        int WeightedRandomBooster()
        {
            float randomValue = Random.Range(0f, boosterWeightSum);
            float cumulativeWeight = 0f;

            for (int i = 0; i < boostersToSpawn.Length; i++)
            {
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

            Boosters boosterToSpawn = boostersToSpawn[WeightedRandomBooster()];

            float spawnAngle = Random.Range(this.GetAngleLimits().lowerBound, this.GetAngleLimits().upperBound);

            GameObject boosterInstance = Instantiate(boosterToSpawn.boosterPrefab, this.GetSpawnPosition(), Quaternion.Euler(0f, 0f, spawnAngle * Mathf.Rad2Deg), transform);
            BoosterController boosterObject = boosterInstance.GetComponent<BoosterController>();
            boosterObject.Init(boosterSpeed);

            StartCoroutine(SpawnBoosters());
        }
    }   
}
