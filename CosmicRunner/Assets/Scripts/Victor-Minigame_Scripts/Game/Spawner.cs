using UnityEngine;

namespace AB
{
    [System.Serializable]
    public struct SpawnInterval
    {
        public float variance;
        public float[] interval;
    }

    // Implementación base de los spawners
    public class Spawner : MonoBehaviour
    {
        // El intervalo de spawn, con una variación aleatoria
        public SpawnInterval spawnInterval = new SpawnInterval { variance = 0.1f, interval = new float[] { 1.0f, 0.3f } };
        // De donde se va a spawnear
        private Vector2 spawnPosition;
        // Y en que angulos puede spawnear, en radianes
        public AngleBounds angleLimits = new AngleBounds { lowerBound = 2.6f, upperBound = 3.1f };
        private float gameDuration;


        protected void Init(SpawnInterval spawnInterval, AngleBounds angleLimits, float gameDuration)
        {
            this.spawnInterval = spawnInterval;
            this.gameDuration = gameDuration;
            this.angleLimits = angleLimits;
            spawnPosition = transform.position;

        }

        protected float GetGameDuration()
        {
            return gameDuration;
        }

        protected AngleBounds GetAngleLimits()
        {
            return angleLimits;
        }

        protected Vector2 GetSpawnPosition()
        {
            return spawnPosition;
        }

        // Interpola el intervalo de spawn
        // Para que aparezcan más enemigos a medida que avanza el juego
        protected float InterpolateSpawnInterval(float x)
        {
            // Usando la formula de interpolación lineal
            // y = y0 + ( (x - x0) * (y1 - y0) ) / (x1 - x0)
            // Donde:
            // x0 = 0 (tiempo inicial)
            // x1 = gameDuration (tiempo final)
            // y0 = spawnInterval.interval[0] (intervalo inicial)
            // y1 = spawnInterval.interval[1] (intervalo final)
            float interpolatedValue = spawnInterval.interval[0] + ( (x) * (spawnInterval.interval[1] - spawnInterval.interval[0]) ) / gameDuration;
            return interpolatedValue;
        }

        // Calcula el intervalo de spawn, usando la interpolación
        // Y agregando una variación aleatoria
        protected float CalculateSpawnInterval()
        {
            // Calcula el intervalo de spawn, usando la interpolación
            // Y agregando una variación aleatoria
            float interval = InterpolateSpawnInterval(GameController.Instance.GetElapsedTime());
            interval += Random.Range(-spawnInterval.variance, spawnInterval.variance);

            return interval;
        }
    }   
}
