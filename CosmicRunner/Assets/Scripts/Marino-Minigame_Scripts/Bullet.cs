using UnityEngine;
using UnityEngine.Tilemaps;

namespace MECS
{
    // Proyectil disparado por el jugador
    public class Bullet : MonoBehaviour
    {
        // Velocidad constante del proyectil
        [SerializeField] private float travelSpeed = 7f;
        // Puntos que da destruir un enemigo
        [SerializeField] private int killScoreValue = 5;

        // Rigido que usamos para empujar la bala en linea recta
        private Rigidbody2D projectileBody;

        // Prepara fisica y collider del proyectil al aparecer
        private void Awake()
        {
            // Buscamos el Rigidbody2D que movera la bala
            projectileBody = GetComponent<Rigidbody2D>();

            // Convertimos el rigidbody en cinemático para controlar nosotros la velocidad
            if (projectileBody != null)
            {
                projectileBody.bodyType = RigidbodyType2D.Kinematic;
                projectileBody.gravityScale = 0f;
                projectileBody.linearVelocity = transform.right * travelSpeed;
            }

            // El collider del proyectil funciona como trigger para no frenarlo al chocar
            Collider2D projectileCollider = GetComponent<Collider2D>();
            if (projectileCollider != null)
            {
                projectileCollider.isTrigger = true;
            }
        }

        // Reproducimos el sonido de disparo al aparecer la bala
        private void Start()
        {
            if (GameControl.Instance != null && GameControl.Instance.sfxManager != null)
            {
                GameControl.Instance.sfxManager.PlayShootSound();
            }
        }

        // Mantiene la velocidad para que no la cambien otras interacciones
        private void Update()
        {
            // Si falta el rigidbody, no hay nada que actualizar
            if (projectileBody == null)
            {
                return;
            }

            // El proyectil siempre avanza hacia su frente local
            projectileBody.linearVelocity = transform.right * travelSpeed;
        }

        // Reacciona al tocar enemigos, cajas o paredes
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Sin collider no hay nada que procesar
            if (collision == null)
            {
                return;
            }

            // Si pega a un enemigo, lo elimina y da puntos
            if (collision.CompareTag("Enemy"))
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
                if (GameControl.Instance != null)
                {
                    GameControl.Instance.AddScore(killScoreValue);

                    if (GameControl.Instance.sfxManager != null)
                    {
                        GameControl.Instance.sfxManager.PlayGoodSound();
                    }
                }
            }

            // Si toca una caja, se destruye para no atravesarla
            if (collision.CompareTag("Box")) {
                Destroy(gameObject);
            }

            // Si choca con una pared del tilemap, tambien se destruye
            if (collision.GetComponent<TilemapCollider2D>() != null)
            {
                Destroy(gameObject);
            }
        }
    }
}

