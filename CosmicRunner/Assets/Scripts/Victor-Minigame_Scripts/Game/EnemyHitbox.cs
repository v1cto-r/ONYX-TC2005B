using UnityEngine;

namespace AB {
    // Controla el hitbox de los enemigos, que es lo que detecta las balas
    public class EnemyHitbox : MonoBehaviour
    {
        // Como el hitbox está dentro del sprite del enemigo
        // El cual está como child del enemigo
        // Se ocupa otro script que afecta directamente al child
        // Para revisar las colisiones
        private EnemyController enemyController;

        private void Awake()
        {
            // Para poder llamar take damage
            enemyController = GetComponentInParent<EnemyController>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                // Daño y destrucción de la bala
                enemyController?.TakeDamage();
                Destroy(other.gameObject);
            }
        }
    }
}