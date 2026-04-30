using UnityEngine;

namespace JorgeGame
{
public class Coin : MonoBehaviour
{
    // valor que suma la moneda
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // si el jugador la toca, suma moneda y se destruye
        if (collision.CompareTag("Player"))
        {
            GameControl.Instance.AddCoin(value);
            SFXManager.instance.PlaySFX(SFXManager.instance.coinSound, 0.4f);
            Destroy(gameObject);
        }
    }
}
}