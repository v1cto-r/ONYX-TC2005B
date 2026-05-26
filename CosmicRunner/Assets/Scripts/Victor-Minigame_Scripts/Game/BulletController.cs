using UnityEngine;

namespace AB {
    public class BulletController : MonoBehaviour
    {
        public float BulletSpeed = 3f;

        void FixedUpdate()
        {
            transform.position += transform.up * BulletSpeed * Time.deltaTime;

            // Si la bala se sale de la pantalla, destruirla para no acumular objetos
            if (Mathf.Abs(transform.position.x) > 20f || Mathf.Abs(transform.position.y) > 20f)
            { 
                Destroy(gameObject);
            }
        }
    }
}