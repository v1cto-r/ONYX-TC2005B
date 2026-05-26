using UnityEngine;

namespace AB
{
    public enum HitboxType { Hard, Soft }
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
            if (other.gameObject.CompareTag("Enemy"))
            {
                if (hitboxType == HitboxType.Hard)
                {
                    shipController?.TakeDamage();
                }
                else
                {
                    shipController?.TakeSoftDamage();
                }

                Destroy(other.gameObject.GetComponentInParent<EnemyController>().gameObject);
            }
        }
    }
}
