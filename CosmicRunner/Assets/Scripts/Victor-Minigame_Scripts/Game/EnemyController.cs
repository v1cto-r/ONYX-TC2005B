using UnityEngine;

namespace AB {
    // Controla a todos los enemigos 
    public class EnemyController : MonoBehaviour
    {
        // Check de si ya chocó porque se triggereaba 2 veces
        private bool hasCollided = false;
        // Referente a cuantos hits aguanta el enemigo, se destruye a 0
        private int hardness;
        // Velocidad del enemigo
        private float speed;
        // Referencia del sprite child para rotarlo
        private Transform spriteTransform;
        // Si rotar en sentido horario o antihorario
        private int rotateDirection;
        // Que tanto va a rotar
        private float rotateMultiplier;

        // El init lo va a mandar a llamar el constructor que va a ser el spawner
        public void Init(int hardness, float speed, Transform spriteTransform)
        {
            this.hardness = hardness;
            this.speed = speed;
            this.spriteTransform = spriteTransform;
            rotateDirection = Random.value < 0.5f ? -1 : 1;
            rotateMultiplier = Random.Range(3f, 7f);
        }


        void FixedUpdate()
        {
            // Lo mueve hacia donde apunta
            transform.position += transform.right * speed * Time.deltaTime;
            
            // Gira el child del sprite
            if (spriteTransform)
                spriteTransform.Rotate(0f, 0f, speed * rotateMultiplier * Time.deltaTime * rotateDirection);
        }

        // Toma daño y se destruye
        public void TakeDamage()
        {
            hardness--;
            if (hardness <= 0)
            {
                Destroy(gameObject);
            }
        }

        // Mantiene track de si ya chocó para no triggerear 2 veces
        public bool CheckHasCollided()
        {
            if (hasCollided)
            {
                return true;
            }

            hasCollided = true;

            Collider2D collider = GetComponentInChildren<Collider2D>();
            collider.enabled = false;

            return false;
        }
    }
}