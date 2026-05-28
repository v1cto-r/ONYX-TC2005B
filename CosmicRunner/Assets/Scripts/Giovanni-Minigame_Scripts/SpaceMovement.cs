using UnityEngine;
namespace Gio.Minigame{

[RequireComponent(typeof(Rigidbody2D))]
public class DerivaEspacial : MonoBehaviour
{
    [Header("Velocidad de cruce")]
    public float velocidadMinima = 3f;
    public float velocidadMaxima = 8f;

    [Tooltip("Tiempo en segundos antes de que el objeto se elimine para no saturar la memoria")]
    public float tiempoDeVida = 15f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // Configuraciones de seguridad para el espacio
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;

        // 1. Encontramos dónde está el centro de la pantalla (la cámara)
        Vector3 posicionCamara = Camera.main.transform.position;
        
        // 2. Calculamos la dirección desde nuestro punto de nacimiento hacia la cámara
        Vector2 direccionHaciaCentro = (posicionCamara - transform.position).normalized;
        
        // 3. Le sumamos un poco de "ruido" aleatorio para que no todos pasen EXACTAMENTE por el centro pixel a pixel
        Vector2 variacionAleatoria = Random.insideUnitCircle * 0.4f;
        Vector2 direccionFinal = (direccionHaciaCentro + variacionAleatoria).normalized;

        // 4. Calculamos una velocidad aleatoria y empujamos el objeto
        float velocidadReal = Random.Range(velocidadMinima, velocidadMaxima);
        rb.linearVelocity = direccionFinal * velocidadReal;

        // 5. Destrucción programada: si el jugador no lo destruye ni lo recoge, el objeto se borra al alejarse
        Destroy(gameObject, tiempoDeVida);
    }
}
}