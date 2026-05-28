using UnityEngine;

namespace AB
{
    public enum HitboxType { Hard, Soft }

    // Como hay dos tipos de hitbox se manejan adentro de un child de la nave
    // Dependiendo del tipo del hitbox, se llama a una función diferente en el ship controller
    public class ShipHitbox : MonoBehaviour
    {
        private ShipControlller shipController;
        public HitboxType hitboxType;

        void Awake()
        {
            shipController = GetComponentInParent<ShipControlller>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Si no es enemigo ignorar
            if (!other.gameObject.CompareTag("Enemy"))
            {
                return;
            }

            EnemyController enemyController = other.GetComponentInParent<EnemyController>();

            // Revisar para evitar doble colisiones
            if (enemyController.CheckHasCollided())
            {
                return;
            }

            // Destruir el enemigo
            Destroy(enemyController.gameObject);

            // Dependiendo de la hitbox accionada
            if (hitboxType == HitboxType.Hard)
            {
                shipController?.TakeDamage();
            }
            else
            {
                shipController?.TakeSoftDamage();
            }
        }
    }
}
