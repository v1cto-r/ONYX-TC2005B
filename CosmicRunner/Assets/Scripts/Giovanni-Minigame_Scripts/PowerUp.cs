using UnityEngine;
namespace Gio.Minigame{
public enum TipoColeccionable 
{ 
    Moneda, 
    Defensa, 
    Ataque, 
    Velocidad 
}

[RequireComponent(typeof(Collider2D))]

public class Coleccionable : MonoBehaviour
{
    public TipoColeccionable tipoItem;
    [Tooltip("Cantidad de monedas que da, o duración en segundos si es un power-up")]
    public float valorODuracion = 5f;
    public Sprite spriteParaUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            
            if (player != null)
            {
                switch (tipoItem)
                {
                    case TipoColeccionable.Moneda:
                        GameManager.Instance.SumarMonedas(1);
                        Debug.Log("Moneda recogida");
                        break;
                    case TipoColeccionable.Defensa:
                        player.ActivarDefensa(valorODuracion);
                        break;
                    case TipoColeccionable.Ataque:
                        player.ActivarAtaque(valorODuracion);
                        break;
                    case TipoColeccionable.Velocidad:
                        player.ActivarVelocidad(valorODuracion);
                        break;
                }
                
                if (tipoItem != TipoColeccionable.Moneda && UIPowerUpContador.Instance != null)
                {
                    UIPowerUpContador.Instance.MostrarContador(spriteParaUI, tipoItem.ToString(), valorODuracion);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
}