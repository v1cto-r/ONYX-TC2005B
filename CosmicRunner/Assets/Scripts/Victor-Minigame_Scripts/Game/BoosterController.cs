using UnityEngine;

namespace AB
{
    public class BoosterController : MonoBehaviour
    {
        private float speed;
        public BoosterType boosterType;

        public void Init(float speed)
        {
            this.speed = speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Collector"))
            {
                Destroy(gameObject);
                GameController.Instance.CollectBooster(boosterType);
            }
        }

        void FixedUpdate()
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }
}
