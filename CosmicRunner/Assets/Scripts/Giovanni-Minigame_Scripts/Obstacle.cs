using UnityEngine;
namespace Gio.Minigame{
[RequireComponent(typeof(Collider2D))]
public class Obstaculo : MonoBehaviour
{
    [Tooltip("Cantidad de disparos necesarios para destruirlo")]
    public int puntosDeVida = 1;

    // Asegúrate de que el collider tenga activado "Is Trigger"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            puntosDeVida--;
            
            Destroy(collision.gameObject); 

            if (puntosDeVida <= 0)
            {
                // Opcional: Aquí podrías instanciar partículas de explosión
                Destroy(gameObject);
            }
        }
        // Si choca con el jugador, le hace daño y se destruye (opcional, dependiendo de tu diseño)
        else if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RecibirDano(1);
            }
            Destroy(gameObject);
        }
    }
}
}
