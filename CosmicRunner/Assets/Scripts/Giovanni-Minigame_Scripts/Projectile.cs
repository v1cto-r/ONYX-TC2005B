using UnityEngine;
namespace Gio.Minigame{
public class ProyectilBasico : MonoBehaviour
{
    public float tiempoDeVida = 3f;
    public int danoBala = 1; 

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            Obstaculo obstaculo = collision.GetComponent<Obstaculo>();
            if (obstaculo != null)
            {
                obstaculo.RecibirDanoProyectil(danoBala);
            }
            
            Destroy(gameObject); //proyectil
        }
    }
}
}