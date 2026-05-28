using UnityEngine;

namespace AB
{
    // Destuyen jeje
    // Para cualquier enemigo que lo toque, evita acumular objetos
    public class Destroyers : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(collision.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(collision.gameObject);
        }
    }
}
