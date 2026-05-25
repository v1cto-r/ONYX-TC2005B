using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyShip : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadEmbiste = 20f;
    public float tiempoDeVida = 5f; 
    public int daño = 2; 

    [Header("Impacto (Knockback)")]
    [Tooltip("Fuerza con la que el enemigo empuja al jugador al chocar")]
    public float fuerzaEmpuje = 15f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        Destroy(gameObject, tiempoDeVida);
    }

    void FixedUpdate()
    {
        // Como rotamos el prefab en Z, su nariz ahora es "transform.up"
        // Si el enemigo viaja de derecha a izquierda, asegúrate de que la rotación Z lo haga apuntar bien.
        rb.linearVelocity = transform.up * velocidadEmbiste; 
    }

    // Usamos OnTriggerEnter2D porque ahora la nave es un Trigger (Atraviesa)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Buscamos el Rigidbody del jugador para empujarlo
            Rigidbody2D rbJugador = collision.GetComponent<Rigidbody2D>();
            
            if (rbJugador != null)
            {
                // 2. Calculamos la dirección del empuje (Desde el enemigo hacia el jugador)
                Vector2 direccionEmpuje = (collision.transform.position - transform.position).normalized;
                
                // 3. Aplicamos un impulso violento en esa dirección
                rbJugador.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode2D.Impulse);
            }

            // Aquí llamarías al GameManager para restar vidas.
            Debug.Log("Jugador golpeado. -2 Vidas. Aplicando Knockback.");
        }
    }
}