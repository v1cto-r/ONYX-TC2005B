using UnityEngine;

namespace AB {
    public class EnemyHitbox : MonoBehaviour
    {
        private EnemyController enemyController;

        private void Awake()
        {
            enemyController = GetComponentInParent<EnemyController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                enemyController?.TakeDamage();
                Destroy(other.gameObject);
            }
        }
    }
}