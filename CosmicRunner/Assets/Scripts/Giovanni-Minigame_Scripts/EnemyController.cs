using UnityEngine;
namespace Gio.Minigame{
public class EnemyShip : MonoBehaviour
{
    public float velocidadEmbiste = 20f;
    public float tiempoDeVida = 5f; 
    public int dano = 2; 
    public float fuerzaEmpuje = 15f;
    private Rigidbody2D rb;
    private Transform camaraTransform;
    private float offsetY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        Destroy(gameObject, tiempoDeVida);

        camaraTransform = Camera.main.transform;
        
        offsetY = transform.position.y - camaraTransform.position.y;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.up * velocidadEmbiste; 

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

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RecibirDano(dano);
            }
        }
    }
}
}