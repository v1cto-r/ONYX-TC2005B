using UnityEngine;

namespace AB
{
    public class BoosterController : MonoBehaviour
    {
        // Velocidad a la cual va a moverse el booster
        private float speed;

        // El tipo de booster, para definir el efecto que va a tener al ser recogido
        public BoosterType boosterType;

        public void Init(float speed)
        {
            this.speed = speed;
        }

        // Si el booster colisiona con el collector, se destruye 
        // y se notifica al GameController para que aplique el efecto
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Collector"))
            {
                Destroy(gameObject);
                GameController.Instance.CollectBooster(boosterType);
            }
        }

        // Mover el booster 
        void FixedUpdate()
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }
}
