using UnityEngine;
namespace Gio.Minigame{
public class ProyectilBasico : MonoBehaviour
{
    public float tiempoDeVida = 1f;

    void Start()
    {
        // El proyectil se destruirá automáticamente tras X segundos si no impacta con nada
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el proyectil golpea un obstáculo (Fase 3), se destruirá.
        if (collision.CompareTag("Obstaculo"))
        {
            // Aquí se restará vida al obstáculo en la siguiente fase
            Destroy(gameObject);
        }
    }
}
}