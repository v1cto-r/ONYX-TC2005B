using UnityEngine;

namespace AB {
    // Controla a todos los enemigos 
    public class EnemyController : MonoBehaviour
    {
        private bool hasCollided = false;
        private int hardness;
        private float speed;
        private Transform spriteTransform;
        private int rotateDirection;
        private float rotateMultiplier;

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
            transform.position += transform.right * speed * Time.deltaTime;
            
            if (spriteTransform)
                spriteTransform.Rotate(0f, 0f, speed * rotateMultiplier * Time.deltaTime * rotateDirection);
        }

        public void TakeDamage()
        {
            hardness--;
            if (hardness <= 0)
            {
                Destroy(gameObject);
            }
        }

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