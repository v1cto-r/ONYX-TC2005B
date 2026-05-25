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
    public float fuerzaEmpuje = 15f;

    private Rigidbody2D rb;
    
    // VARIABLES PARA MANTENER LA BARRIDA ESTABLE
    private Transform camaraTransform;
    private float offsetY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        Destroy(gameObject, tiempoDeVida);

        // 1. Guardamos la referencia de la cámara principal
        camaraTransform = Camera.main.transform;
        
        // 2. Calculamos a qué distancia vertical (Y) nació el enemigo respecto a la cámara
        offsetY = transform.position.y - camaraTransform.position.y;
    }

    void FixedUpdate()
    {
        // 1. Aplicamos la velocidad horizontal normal
        rb.linearVelocity = transform.up * velocidadEmbiste; 

        // 2. TRUCO DE PANTALLA: Forzamos la posición Y para que viaje "pegado" a la cámara
        Vector2 posicionCorregida = rb.position;
        posicionCorregida.y = camaraTransform.position.y + offsetY;
        rb.position = posicionCorregida;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.GetComponent<Rigidbody2D>();
            
            if (rbJugador != null)
            {
                Vector2 direccionEmpuje = (collision.transform.position - transform.position).normalized;
                rbJugador.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode2D.Impulse);
            }

            Debug.Log("Jugador golpeado. -2 Vidas. Aplicando Knockback.");
        }
    }
}