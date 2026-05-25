using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyShip : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadEmbiste = 20f;
    public float tiempoDeVida = 5f; // Tiempo antes de autodestruirse si no choca
    public int daño = 2; // Vidas que quita al jugador

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // El Rigidbody2D debe estar en modo Kinematic para que no lo afecten colisiones físicas 
        // y se mueva como una bala, o Dynamic con masa alta y gravity 0.
        rb.gravityScale = 0f;
        
        // Destruir por seguridad después de X segundos
        Destroy(gameObject, tiempoDeVida);
    }

    void FixedUpdate()
    {
        // Se mueve horizontalmente hacia la izquierda (asumiendo que aparece en el lado derecho)
        // Cambia transform.right por -transform.right dependiendo de hacia dónde mire tu sprite
        rb.linearVelocity = -transform.right * velocidadEmbiste; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Asumiendo que tu nave tiene el tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Aquí llamarás al GameManager para restar las 2 vidas
            // GameManager.Instance.RestarVidas(daño);
            
            Debug.Log("Jugador golpeado. -2 Vidas.");
        }
    }
}