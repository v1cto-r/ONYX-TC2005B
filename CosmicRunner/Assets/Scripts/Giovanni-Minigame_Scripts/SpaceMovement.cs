using UnityEngine;
namespace Gio.Minigame{
public class DerivaEspacial : MonoBehaviour
{
    public float velocidadMinima = 3f;
    public float velocidadMaxima = 8f;
    public float tiempoDeVida = 15f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;

        Vector3 posicionCamara = Camera.main.transform.position;

        Vector2 direccionHaciaCentro = (posicionCamara - transform.position).normalized;

        Vector2 variacionAleatoria = Random.insideUnitCircle * 0.4f;
        Vector2 direccionFinal = (direccionHaciaCentro + variacionAleatoria).normalized;

        float velocidadReal = Random.Range(velocidadMinima, velocidadMaxima);
        rb.linearVelocity = direccionFinal * velocidadReal;
        Destroy(gameObject, tiempoDeVida);
    }
}
}