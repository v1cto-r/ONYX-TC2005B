using UnityEngine;
namespace Gio.Minigame{
public enum TipoColeccionable 
{ 
    Moneda, 
    Defensa, 
    Ataque, 
    Velocidad 
}
public class Coleccionable : MonoBehaviour
{
    public TipoColeccionable tipoItem;
    public float duracion = 5f; 
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
                        GameManager.Instance.SumarMonedas(5);
                        if (SFXManager.Instance != null) SFXManager.Instance.PlayCoinSound();
                        Debug.Log("Moneda recogida");
                        break;
                    case TipoColeccionable.Defensa:
                        if (SFXManager.Instance != null) SFXManager.Instance.PlayPowerUpSound();
                        player.ActivarDefensa(duracion);
                        break;
                    case TipoColeccionable.Ataque:
                        if (SFXManager.Instance != null) SFXManager.Instance.PlayPowerUpSound();
                        player.ActivarAtaque(duracion);
                        break;
                    case TipoColeccionable.Velocidad:
                        if (SFXManager.Instance != null) SFXManager.Instance.PlayPowerUpSound();
                        player.ActivarVelocidad(duracion);
                        break;
                }
                
                if (tipoItem != TipoColeccionable.Moneda && UIPowerUpContador.Instance != null)
                {
                    UIPowerUpContador.Instance.MostrarContador(spriteParaUI, tipoItem.ToString(), duracion);
                }
            }
            
            Destroy(gameObject);
        }
    }
}
}