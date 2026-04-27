using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // si el jugador toca la zona de muerte
        if (collision.CompareTag("Player"))
        {
            // pierde una vida
            GameControl.Instance.SpendLives();

            // vuelve al ultimo checkpoint guardado
            collision.transform.position = CheckpointManager.instance.respawnPoint;
        }
    }
}