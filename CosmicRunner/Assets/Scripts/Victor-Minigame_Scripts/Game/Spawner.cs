using UnityEngine;

namespace AB
{
    [System.Serializable]
    public struct SpawnInterval
    {
        public float variance;
        public float[] interval;
    }

    public class Spawner : MonoBehaviour
    {
        public SpawnInterval spawnInterval = new SpawnInterval { variance = 0.1f, interval = new float[] { 1.0f, 0.3f } };
        private Vector2 spawnPosition;
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

        protected float InterpolateSpawnInterval(float x)
        {
            float interpolatedValue = spawnInterval.interval[0] + ( (x) * (spawnInterval.interval[1] - spawnInterval.interval[0]) ) / gameDuration;
            return interpolatedValue;
        }

        protected float CalculateSpawnInterval()
        {
            float interval = InterpolateSpawnInterval(GameController.Instance.GetElapsedTime());
            interval += Random.Range(-spawnInterval.variance, spawnInterval.variance);

            return interval;
        }
    }   
}
