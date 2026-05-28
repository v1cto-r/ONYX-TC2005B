using UnityEngine;
namespace Gio.Minigame{
public class Obstaculo : MonoBehaviour
{
    public int puntosDeVida = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RecibirDano(1);
            }
            Destroy(gameObject);
        }
    }
    
    public void RecibirDanoProyectil(int cantidad)
    {
        puntosDeVida -= cantidad;
        if (puntosDeVida <= 0)
        {
            Destroy(gameObject);
        }
    }
}
}
