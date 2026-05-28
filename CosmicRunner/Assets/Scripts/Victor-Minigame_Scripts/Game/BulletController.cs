using UnityEngine;

namespace AB {
    public class BulletController : MonoBehaviour
    {
        // Velocidad a la que se mueve la bala
        public float BulletSpeed = 3f;

        // Mover la bala hacia adelante
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