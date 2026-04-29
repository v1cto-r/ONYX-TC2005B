using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // si el jugador cae aqui, pierde una vida y regresa al checkpoint
        if (collision.CompareTag("Player"))
        {
            GameControl.Instance.SpendLives();
            collision.transform.position = SpawnPoint.instance.respawnPoint;
        }
    }
}